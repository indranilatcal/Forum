using MVCForum.Domain.DomainModel;
using MVCForum.Domain.DomainModel.Enums;
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Application;
using MVCForum.Website.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Security;

namespace MVCForum.Website.Controllers
{
	public class RegistrationController : ApiController
	{
		private readonly ISettingsService _settingsService;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly ILocalizationService _localizationService;
		private readonly IBannedEmailService _bannedEmailService;
		private readonly IBannedWordService _bannedWordService;
		private readonly IMembershipService _membershipService;
		private readonly ILoggingService _loggingService;

		public RegistrationController(ISettingsService settingsService, IUnitOfWorkManager unitOfWorkManager,
			ILocalizationService localizationService, IBannedEmailService bannedEmailService, IBannedWordService bannedWordService,
			IMembershipService membershipService, ILoggingService loggingService)
		{
			_settingsService = settingsService;
			_unitOfWorkManager = unitOfWorkManager;
			_localizationService = localizationService;
			_bannedEmailService = bannedEmailService;
			_bannedWordService = bannedWordService;
			_membershipService = membershipService;
			_loggingService = loggingService;
		}
		public HttpResponseMessage Post(MemberAddViewModel userModel)
		{
			if (_settingsService.GetSettings().SuspendRegistration != true)
			{
				using (_unitOfWorkManager.NewUnitOfWork())
				{
					// First see if there is a spam question and if so, the answer matches
					if (!string.IsNullOrEmpty(_settingsService.GetSettings().SpamQuestion))
					{
						// There is a spam question, if answer is wrong return with error
						if (userModel.SpamAnswer == null || userModel.SpamAnswer.Trim() != _settingsService.GetSettings().SpamAnswer)
						{
							// POTENTIAL SPAMMER!
							ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Error.WrongAnswerRegistration"));
							return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
						}
					}

					// Secondly see if the email is banned
					if (_bannedEmailService.EmailIsBanned(userModel.Email))
					{
						ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Error.EmailIsBanned"));
						return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
					}
				}

				// Standard Login
				userModel.LoginType = LoginType.Standard;

				// Do the register logic
				return MemberRegisterLogic(userModel);

			}

			return Request.CreateResponse(HttpStatusCode.OK);
		}

		private HttpResponseMessage MemberRegisterLogic(MemberAddViewModel userModel)
		{
			using (var unitOfWork = _unitOfWorkManager.NewUnitOfWork())
			{
				var userToSave = new MVCForum.Domain.DomainModel.MembershipUser
				{
					UserName = _bannedWordService.SanitiseBannedWords(userModel.UserName),
					Email = userModel.Email,
					Password = userModel.Password,
					IsApproved = userModel.IsApproved,
					Comment = userModel.Comment,
				};

				var createStatus = _membershipService.CreateUser(userToSave);
				if (createStatus != MVCForum.Domain.DomainModel.MembershipCreateStatus.Success)
				{
					ModelState.AddModelError(string.Empty, _membershipService.ErrorCodeToString(createStatus));
					return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
				}
				else
				{
					// See if this is a social login and we have their profile pic
					if (!string.IsNullOrWhiteSpace(userModel.SocialProfileImageUrl))
					{
						// We have an image url - Need to save it to their profile
						var image = AppHelpers.GetImageFromExternalUrl(userModel.SocialProfileImageUrl);

						// Set upload directory - Create if it doesn't exist
						var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.UploadFolderPath, userToSave.Id));
						if (!Directory.Exists(uploadFolderPath))
						{
							Directory.CreateDirectory(uploadFolderPath);
						}

						// Get the file name
						var fileName = Path.GetFileName(userModel.SocialProfileImageUrl);

						// Create a HttpPostedFileBase image from the C# Image
						using (var stream = new MemoryStream())
						{
							image.Save(stream, ImageFormat.Jpeg);
							stream.Position = 0;
							HttpPostedFileBase formattedImage = new MemoryFile(stream, "image/jpeg", fileName);

							// Upload the file
							var uploadResult = AppHelpers.UploadFile(formattedImage, uploadFolderPath, _localizationService);

							// Don't throw error if problem saving avatar, just don't save it.
							if (uploadResult.UploadSuccessful)
							{
								userToSave.Avatar = uploadResult.UploadedFileName;
							}
						}

					}

					// Store access token for social media account in case we want to do anything with it
					if (userModel.LoginType == LoginType.Facebook)
					{
						userToSave.FacebookAccessToken = userModel.UserAccessToken;
					}
					if (userModel.LoginType == LoginType.Google)
					{
						userToSave.GoogleAccessToken = userModel.UserAccessToken;
					}

					try
					{
						userToSave.IsApproved = userModel.IsApproved;
						unitOfWork.Commit();
						return Request.CreateResponse(HttpStatusCode.OK, userToSave.Id);
					}
					catch (Exception ex)
					{
						unitOfWork.Rollback();
						_loggingService.Error(ex);
						FormsAuthentication.SignOut();
						ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Errors.GenericMessage"));
						return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
					}
				}
			}
		}
	}
}
