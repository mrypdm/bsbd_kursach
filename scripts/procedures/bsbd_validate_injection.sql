create function bsbd_validate_injection(@str nvarchar(100))
returns int as
begin
    if @str is null
    begin
        return 2
    end

    if charindex('[', @str) > 0 or charindex(']', @str) > 0
       or charindex('"', @str) > 0 or charindex(';', @str) > 0
       or charindex('''', @str) > 0
    begin
        return 1
    end
 
    return 0
end
