using System;
using System.ComponentModel.DataAnnotations;

namespace DireDawaHub.Models;

public class IdDocumentImage
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    public string FilePath { get; set; } = string.Empty;
    
    public long FileSize { get; set; }
    
    public string? ContentType { get; set; }
    
    public DateTime UploadedAt { get; set; } = DateTime.Now;
    
    // Visual verification status by admin
    public bool IsVisuallyVerified { get; set; } = false;
    
    public string? VisualVerificationNotes { get; set; }
    
    public DateTime? VerifiedAt { get; set; }
    
    public string? VerifiedBy { get; set; }
    
    // Selfie photo for face matching (future Phase 3)
    public string? SelfieFilePath { get; set; }
    
    public bool FaceMatchCompleted { get; set; } = false;
    
    public double? FaceMatchConfidence { get; set; }
    
    // OCR extracted data (future Phase 3)
    public string? ExtractedIdNumber { get; set; }
    
    public string? ExtractedFullName { get; set; }
    
    public string? ExtractedDepartment { get; set; }
    
    // Document type
    public IdDocumentType DocumentType { get; set; } = IdDocumentType.WorkId;
}

public enum IdDocumentType
{
    WorkId,
    GovernmentId,
    MunicipalEmployeeCard,
    Passport,
    DriverLicense
}
