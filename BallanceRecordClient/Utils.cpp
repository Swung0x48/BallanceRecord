#include "Utils.h"
#include "sha256.h"
#include <vector>

std::string Utils::Hash(std::ifstream& fs)
{
	const int BUF_SIZE = 256;
	SHA256 sha256;
	std::vector<char> buffer(BUF_SIZE, 0);
	while (!fs.eof())
	{
		fs.read(buffer.data(), buffer.size());
		std::streamsize readSize = fs.gcount();
		sha256.add(buffer.data(), readSize);
	}
	return sha256.getHash();
}