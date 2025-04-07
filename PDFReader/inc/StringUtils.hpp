#ifndef STRING_UTILS_HPP__
#define STRING_UTILS_HPP__

#include <string>
#include <vector>

std::string Replace(std::string str, const std::string& str1, const std::string& replaceBy);
void Split(std::string str, std::string delimiter, std::vector<std::string>& tokens);
void RTrim(std::string& s);
void LTrim(std::string& s);
void Trim(std::string& s);
void ToLower(std::string& s);
void ToUpper(std::string& s);

#endif