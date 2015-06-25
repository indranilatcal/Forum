// Custom Code:

using MVCForum.Domain.Interfaces.Services;
using MVCForum.Domain.Interfaces.UnitOfWork;
using System.Web.Http;

namespace MVCForum.Website.Controllers.ApiControllers
{
	public abstract class BaseApiController : ApiController
	{
		protected readonly ISettingsService SettingsService;
		protected readonly ILoggingService LoggingService;
		protected readonly ILocalizationService LocalizationService;
		protected readonly IRoleService RoleService;
		protected readonly IUnitOfWorkManager UnitOfWorkManager;

		public BaseApiController(
			ISettingsService settingsService,
			ILoggingService loggingService,
			ILocalizationService localizationService,
			IRoleService roleService,
			IUnitOfWorkManager unitOfWorkManager)
		{
			SettingsService = settingsService;
			LoggingService = loggingService;
			LocalizationService = localizationService;
			RoleService = roleService;
			UnitOfWorkManager = unitOfWorkManager;
		}
	}
}