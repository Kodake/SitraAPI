//using MediatR;

//namespace SITRA.Core.Mediator.Commands.AdministrativeTools.Roles
//{
//    public record SeedPermissionsCommand : IRequest<bool>;

//    public class SeedPermissionsCommandHandler : IRequestHandler<SeedPermissionsCommand, bool>
//    {
//        private readonly IPermissionRepository _permissionRepository;

//        public SeedPermissionsCommandHandler(IPermissionRepository permissionRepository)
//        {
//            _permissionRepository = permissionRepository;
//        }

//        public async Task<bool> Handle(SeedPermissionsCommand request, CancellationToken cancellationToken)
//        {
//            await _permissionRepository.SeedPermissionsAsync(cancellationToken);
//            return true;
//        }
//    }
//}
