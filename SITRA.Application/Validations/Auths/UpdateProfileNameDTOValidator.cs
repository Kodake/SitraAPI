using FluentValidation;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using SITRA.Core.DTOs.Auths;
using SITRA.Core.Helpers;
using System;
using System.IO;
using System.Security.Cryptography;
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

                        using var certMs = new MemoryStream(certBytes);
                        var store = new Pkcs12StoreBuilder().Build();
                        store.Load(certMs, dto.CertificatePassword.ToCharArray());

                        DateTime? notAfter = null;
                        DateTime? notBefore = null;

                        foreach (string alias in store.Aliases)
                        {
                            var entry = store.GetCertificate(alias);
                            if (entry?.Certificate != null)
                            {
                                notAfter = entry.Certificate.NotAfter;
                                notBefore = entry.Certificate.NotBefore;
                                break;
                            }
                        }

                        if (notAfter.HasValue && notAfter.Value < DateTime.UtcNow)
                        {
                            context.AddFailure("Certificate", Utilities.CertificateExpiredMessage);
                        }

                        if (notBefore.HasValue && notBefore.Value > DateTime.UtcNow)
                        {
                            context.AddFailure("Certificate", Utilities.CertificateNotYetValidMessage);
                        }
                    }
                    catch (InvalidCipherTextException)
                    {
                        context.AddFailure("CertificatePassword", Utilities.CertificatePasswordIncorrectMessage);
                    }
                    catch (IOException ex) when (ex.Message.Contains("MAC invalid", StringComparison.OrdinalIgnoreCase) ||
                                                 ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase))
                    {
                        context.AddFailure("CertificatePassword", Utilities.CertificatePasswordIncorrectMessage);
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("password", StringComparison.OrdinalIgnoreCase) ||
                            ex.Message.Contains("MAC", StringComparison.OrdinalIgnoreCase))
                        {
                            context.AddFailure("CertificatePassword", Utilities.CertificatePasswordIncorrectMessage);
                        }
                        else
                        {
                            context.AddFailure("Certificate", Utilities.CertificateInvalidFormatMessage);
                        }
                    }
                }
            });
        }
    }
}
