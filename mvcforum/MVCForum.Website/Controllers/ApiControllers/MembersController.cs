// Custom Code:
using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Application;
using MVCForum.Website.ViewModels;
using MVCForum.Website.ViewModels.Mapping;
using System;
using System.Linq;
using System.Web.Http;

namespace MVCForum.Website.Controllers.ApiControllers
{
	[RoutePrefix("api/Members")]
	public class MembersController : BaseApiController
	{
		private readonly ITopicService _topicService;
		private readonly IMembershipService _membershipService;

		public MembersController(
			IUnitOfWorkManager unitOfWorkManager,
			ITopicService topicService,
			ISettingsService settingsService,
			IRoleService roleService,
			IMembershipService membershipService,
			ILoggingService loggingService,
			ILocalizationService localizationService)
			: base(settingsService, loggingService, localizationService, roleService, unitOfWorkManager)
		{
			_topicService = topicService;
			_membershipService = membershipService;
		}

		[Route("{id}/TopicsPostedIn")]
		public IHttpActionResult GetTopicsPostedIn(string id)
		{
			try
			{
				var user = _membershipService.GetUserByUniversalId(id);
				if (user == null)
					return NotFound();
				int pageIndex = 1;

				// Get the topics
				var topics = _topicService.GetMembersActivity(pageIndex,
								SettingsService.GetSettings().TopicsPerPage,
								SiteConstants.MembersActivityListSize,
								user.Id);

				// Get the Topic View Models
				var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics,
					RoleService, user.Roles.FirstOrDefault(), user, SettingsService.GetSettings());

				// create the view model
				var viewModel = new PostedInViewModel
				{
					Topics = topicViewModels,
					PageIndex = pageIndex,
					TotalCount = topics.TotalCount,
					TotalPages = topics.TotalPages
				};

				return Ok(viewModel);
			}
			catch (Exception ex)
			{
				LoggingService.Error(ex);
				ModelState.AddModelError(string.Empty, LocalizationService.GetResourceString("Errors.GenericMessage"));
			}

			return BadRequest(ModelState);
		}
	}
}
