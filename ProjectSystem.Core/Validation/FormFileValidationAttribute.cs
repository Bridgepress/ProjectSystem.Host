using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class FormFileValidationAttribute : ValidationAttribute
{
    public string[] AllowedExtensions { get; set; }
    public long MaxFileSize { get; set; }

    public FormFileValidationAttribute(string[] allowedExtensions, long maxFileSize)
    {
        AllowedExtensions = allowedExtensions;
        MaxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var file = value as IFormFile;

        if (file == null)
        {
            return ValidationResult.Success;
        }


        if (file.Length > MaxFileSize)
        {
            return new ValidationResult($"File size cannot exceed {MaxFileSize / 1024 / 1024} MB.");
        }

        var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
        if (!AllowedExtensions.Contains(extension))
        {
            return new ValidationResult($"This file type is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}");
        }

        return ValidationResult.Success;
    }
}
