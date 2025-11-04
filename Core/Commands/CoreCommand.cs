namespace Core.Commands;

public record CoreCommand<T>
(
    CoreCommandType CommandType,
    T? Param
);
