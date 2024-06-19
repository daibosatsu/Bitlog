using System.Net.Mail;
using System.Text;
using Bitlog.WebApi.Dtos;

namespace Bitlog.WebApi.Validators
{
    public interface IValidator<T> where T : class
    {
        (bool success, string? errorMessage) Validate(T value);

        IValidator<T> SetNext(IValidator<T> validator);
    }

    public class UserValidator : IValidator<UserPayload>
    {
        private IValidator<UserPayload> _validator;

        public IValidator<UserPayload> SetNext(IValidator<UserPayload> validator)
        {
            _validator = validator;
            return _validator;
        }

        public (bool success, string? errorMessage) Validate(UserPayload value)
        {
            if (string.IsNullOrEmpty(value.Email) && string.IsNullOrEmpty(value.PhoneNumber))
            {
                return (false, "Both email and phone number cannot be empty");
            }

            return _validator != null ? _validator.Validate(value) : (true, null);
        }
    }

    public class UserEmailPhoneValidator : IValidator<UserPayload>
    {
        private IValidator<UserPayload>? _validator;

        private readonly IValidator<UserPayload> _emailValidator;
        private readonly IValidator<UserPayload> _phoneValidator;

        public UserEmailPhoneValidator(IValidator<UserPayload> emailValidator, IValidator<UserPayload> phoneValidator)
        {
            _emailValidator = emailValidator;
            _phoneValidator = phoneValidator;
        }

        public IValidator<UserPayload> SetNext(IValidator<UserPayload> validator)
        {
            _validator = validator;
            return _validator;
        }

        public (bool success, string? errorMessage) Validate(UserPayload value)
        {
            var emailValidationResult = _emailValidator.Validate(value);
            var phoneValidationResult = _phoneValidator.Validate(value);

            // email and phone are optional, so if one is empty but the other is success, the model is valid.
            // email validated correctly, but user also input an invalid phoneno
            if (emailValidationResult.success is true
                && (string.IsNullOrEmpty(value.PhoneNumber) is false && phoneValidationResult.success is false))
            {
                return (phoneValidationResult.success, phoneValidationResult.errorMessage);
            }
            else if (phoneValidationResult.success is true
                && (string.IsNullOrEmpty(value.Email) is false && emailValidationResult.success is false))
            {
                return (emailValidationResult.success, emailValidationResult.errorMessage);
            }
            else if (phoneValidationResult.success is false && emailValidationResult.success is false)
            {
                return (false, new StringBuilder()
                    .Append(emailValidationResult.errorMessage)
                    .AppendLine(phoneValidationResult.errorMessage).ToString());
            }

            return _validator != null ? _validator.Validate(value) : (true, null);
        }
    }

    public class UserEmailValidator : IValidator<UserPayload>
    {
        public IValidator<UserPayload> SetNext(IValidator<UserPayload> validator)
        {
            throw new NotImplementedException();
        }

        public (bool success, string? errorMessage) Validate(UserPayload value)
        {
            var userEmail = value.Email;
            if (userEmail?.Length > 256)
            {
                return (false, "Email is too long.");
            }

            if (MailAddress.TryCreate(userEmail, out _) is false)
            {
                return (false, "Email is invalid.");
            }

            return (true, null);
        }
    }

    public abstract class CharacterValidator
    {
        protected bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        protected bool IsLower(char c)
        {
            return c >= 'a' && c <= 'z';
        }

        protected bool IsUpper(char c)
        {
            return c >= 'A' && c <= 'Z';
        }
    }

    public class UserPasswordValidator : CharacterValidator, IValidator<UserPayload>
    {
        const string ValidationErrorMessage = "Password must be at least 8 characters and contains at least one lowercase character, " +
            "at least one uppercase character, at least two digits, at least one symbol.";

        public IValidator<UserPayload> SetNext(IValidator<UserPayload> validator)
        {
            throw new NotImplementedException();
        }

        public (bool success, string? errorMessage) Validate(UserPayload value)
        {
            var pwd = value.Password;
            if (string.IsNullOrEmpty(pwd) || pwd.Length < 8)
            {
                return (false, ValidationErrorMessage);
            }

            var digitCount = 0;
            var lowerCount = 0;
            var upperCount = 0;
            var specialCharCount = 0;

            foreach (var c in pwd)
            {
                if (IsDigit(c))
                {
                    digitCount++;
                }
                else if (IsLower(c))
                {
                    lowerCount++;
                }
                else if (IsUpper(c))
                {
                    upperCount++;
                }
                else
                {
                    specialCharCount++;
                }
            }

            return digitCount >= 2 && lowerCount >= 1 && upperCount >= 1 && specialCharCount >= 1 ?
                (true, null) :
                (false, ValidationErrorMessage);
        }
    }


    public class UserPhoneValidator : CharacterValidator, IValidator<UserPayload>
    {
        const string InvalidPhonenumberLength = "Phone number must contains country code and 9 digits.";

        public IValidator<UserPayload> SetNext(IValidator<UserPayload> validator)
        {
            throw new NotImplementedException();
        }

        public (bool success, string? errorMessage) Validate(UserPayload value)
        {
            var userPhone = value.PhoneNumber;
            if (string.IsNullOrEmpty(userPhone))
            {
                return (false, "Phonenumber must not be empty.");
            }

            foreach (char c in userPhone)
            {
                if (c != '+' && IsDigit(c) is false)
                {
                    return (false, "Phone number can only contains '+' or digits.");
                }
            }

            // 9 digit + 1 to 3 digits for country code
            if (userPhone[0] == '+' && (userPhone.Length > 9 && userPhone.Length < 13))
            {
                return (true, null);
            }
            else if (userPhone[0] == '0')
            {
                if (userPhone[1] == '0' && (userPhone.Length > 10 && userPhone.Length < 14))
                {
                    return (true, null);
                }
                else if (userPhone.Length == 10) // normal phone with leading zero
                {
                    return (true, null);
                }
            }

            return (false, "Invalid phone number length.");
        }
    }
}