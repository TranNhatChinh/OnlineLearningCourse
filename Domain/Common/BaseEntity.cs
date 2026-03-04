namespace Domain.Common;

/// <summary>
/// Base entity with common properties for all entities
/// Includes: Identity, Timestamps, Audit Trail, and Soft Delete
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// User ID who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Indicates whether the entity is soft deleted
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date and time when the entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// User ID who deleted the entity
    /// </summary>
    public string? DeletedBy { get; set; }
}
