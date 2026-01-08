using SampleProject.Application.DTOs;
using SampleProject.Application.Mediator;

namespace SampleProject.Application.Queries.Storage;

public record GetAllStoragesQuery() : IRequest<List<StorageDto>>;
