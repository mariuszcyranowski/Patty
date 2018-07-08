# Patty
If you ever wanted to check if your regexes can match all the data then this tool can help you. Basically it consist of Cli application and library which take two inputs - samples and patterns
## Samples
These are strings which we want to cover with regex patterns
## Patterns (or pattern fragments)
Are list of regexes which are used by this tool to create new ones (compile them). Resulting patterns can consist of these regexes if they match the samples.
## Example
given list of samples:
- aaaa
- abcd
- uwx

and list of patterns:
- [ab]
- [cd]
- x$

we will receive a new list:
- [ab]aaa
- [ab]b[cd]d
- uwx$
