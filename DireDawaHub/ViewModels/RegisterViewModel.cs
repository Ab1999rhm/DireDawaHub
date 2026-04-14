using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using DireDawaHub.Models;

namespace DireDawaHub.ViewModels;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [Display(Name = "Government or NGO Work ID")]
    public string WorkId { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [Display(Name = "ID Document Photo")]
    [Required(ErrorMessage = "Please upload your ID document for verification")]
    public IFormFile IdDocumentPhoto { get; set; }

    [Display(Name = "Selfie with ID (optional)")]
    public IFormFile? SelfieWithId { get; set; }

    [Display(Name = "Document Type")]
    public IdDocumentType DocumentType { get; set; } = IdDocumentType.WorkId;
}
