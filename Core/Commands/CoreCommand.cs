namespace Core.Commands;

public record CoreCommand
(
    CoreCommandType CommandType,
    object? Param = null
);
