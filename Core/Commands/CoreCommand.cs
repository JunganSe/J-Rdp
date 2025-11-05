namespace Core.Commands;

public record CoreCommand
(
    CoreCommandType CommandType
);

public record CoreCommand<T>
(
    CoreCommandType CommandType,
    T? Param
);
