#include "../inc/StringUtils.hpp"

std::string Replace(std::string str, const std::string& str1, const std::string& replaceBy)
{
    size_t start_pos = 0;
    while ((start_pos = str.find(str1, start_pos)) != std::string::npos)
    {
        str.replace(start_pos, str1.length(), replaceBy);
        start_pos += replaceBy.length(); // Handles case where 'to' is a substring of 'from'
    }
    return str;
}


void Split(std::string str, std::string delimiter, std::vector<std::string>& tokens)
{
    tokens.clear();
    std::string s = str;
    size_t pos = 0;
    std::string token;
    while ((pos = s.find(delimiter)) != std::string::npos)
    {
        token = s.substr(0, pos);
        tokens.push_back(token);
        s.erase(0, pos + delimiter.length());
    }
    tokens.push_back(s);
}

void RTrim(std::string& s)
{
    s.erase(std::find_if(s.rbegin(), s.rend(), [](unsigned char ch) {return !std::isspace(ch); }).base(), s.end());
}

void LTrim(std::string& s)
{
    s.erase(s.begin(), std::find_if(s.begin(), s.end(), [](unsigned char ch) {return !std::isspace(ch); }));
}

void Trim(std::string& s)
{
    RTrim(s);
    LTrim(s);
}


void ToLower(std::string& s)
{
    std::transform(s.begin(), s.end(), s.begin(), [](unsigned char c) { return std::tolower(c); });
}

void ToUpper(std::string& s)
{
    std::transform(s.begin(), s.end(), s.begin(), [](unsigned char c) { return std::toupper(c); });
}