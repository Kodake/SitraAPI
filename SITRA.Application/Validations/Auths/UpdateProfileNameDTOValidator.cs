using FluentValidation;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SITRA.Core.Validations.Auths
{
    public class UpdateProfileNameDTOValidator : AbstractValidator<UpdateProfileNameDTO>
    {
        public UpdateProfileNameDTOValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(Utilities.RequiredFieldMessage)
                .MaximumLength(100).WithMessage(Utilities.MaximunLengthMessage);

            RuleFor(x => x.Province)
                .MaximumLength(6).WithMessage(Utilities.MaximunLengthMessage)
                .When(x => !string.IsNullOrEmpty(x.Province));

            RuleFor(x => x.Municipality)
                .MaximumLength(6).WithMessage(Utilities.MaximunLengthMessage)
                .When(x => !string.IsNullOrEmpty(x.Municipality));

            // Custom validation for Certificate and Password
            RuleFor(x => x).CustomAsync(async (dto, context, cancellation) => 
            {
                if (dto.Certificate != null)
                {
                    if (string.IsNullOrEmpty(dto.CertificatePassword))
                    {
                        context.AddFailure("CertificatePassword", Utilities.CertificatePasswordRequiredMessage);
                        return;
                    }

                    try
                    {
                        using var ms = new MemoryStream();
                        await dto.Certificate.CopyToAsync(ms, cancellation);
                        var certBytes = ms.ToArray();

                        using var cert = new X509Certificate2(certBytes, dto.CertificatePassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet);

                        if (cert.NotAfter < DateTime.UtcNow)
                        {
                            context.AddFailure("Certificate", Utilities.CertificateExpiredMessage);
                        }

                        if (cert.NotBefore > DateTime.UtcNow)
                        {
                            context.AddFailure("Certificate", Utilities.CertificateNotYetValidMessage);
                        }
                    }
                    catch (CryptographicException)
                    {
                        context.AddFailure("CertificatePassword", Utilities.CertificatePasswordIncorrectMessage);
                    }
                    catch (Exception)
                    {
                        context.AddFailure("Certificate", Utilities.CertificateInvalidFormatMessage);
                    }
                }
            });
        }
    }
}
