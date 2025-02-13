using System.ComponentModel.DataAnnotations;

namespace _234191F_AS_Assignment1.Validation
{
	public class AllowedExtensionsAttribute: ValidationAttribute
	{
		private readonly string[] _extensions;
		public AllowedExtensionsAttribute(string[] extensions)
		{
			_extensions = extensions;
		}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var file = value as IFormFile;
			if (file != null)
			{
				var extension = Path.GetExtension(file.FileName);
				if (!_extensions.Contains(extension.ToLower()))
				{
					return new ValidationResult($"Only {string.Join(", ", _extensions)} files are allowed.");
				}
			}

			return ValidationResult.Success;
		}
	}
}
