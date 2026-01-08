using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Storage;

public record GetStorageByIdQuery(Guid StorageId) : IRequest<StorageDto?>;
