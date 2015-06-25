using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using MVCForum.Website.Application;
using MVCForum.Website.ViewModels;
using MVCForum.Website.ViewModels.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MVCForum.Website.Controllers.ApiControllers
{
	[RoutePrefix("api/Members")]
	public class MembersController : ApiController
	{
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly ITopicService _topicService;
		private readonly ISettingsService _settingsService;
		private readonly IRoleService _roleService;
		private readonly IMembershipService _membershipService;
		private readonly ILoggingService _loggingService;
		private readonly ILocalizationService _localizationService;

		public MembersController(
			IUnitOfWorkManager unitOfWorkManager,
			ITopicService topicService,
			ISettingsService settingsService,
			IRoleService roleService,
			IMembershipService membershipService,
			ILoggingService loggingService,
			ILocalizationService localizationService)
		{
			_unitOfWorkManager = unitOfWorkManager;
			_topicService = topicService;
			_settingsService = settingsService;
			_roleService = roleService;
			_membershipService = membershipService;
			_loggingService = loggingService;
			_localizationService = localizationService;
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
								_settingsService.GetSettings().TopicsPerPage,
								SiteConstants.MembersActivityListSize,
								user.Id);

				// Get the Topic View Models
				var topicViewModels = ViewModelMapping.CreateTopicViewModels(topics,
					_roleService, user.Roles.FirstOrDefault(), user, _settingsService.GetSettings());

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
				_loggingService.Error(ex);
				ModelState.AddModelError(string.Empty, _localizationService.GetResourceString("Errors.GenericMessage"));
			}

			return BadRequest(ModelState);
		}
	}
}
