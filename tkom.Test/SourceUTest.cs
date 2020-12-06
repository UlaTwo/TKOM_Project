using System;
using Xunit;
using tkom.LexerN;
using System.IO;

namespace tkom.Test
{

    public class StringSourceTest
    {
        [Fact]
        public void StringWithContent_stringSourceReadsFirstCharacter()
        {
            //prepare
            var str = "example";
            //act
            var stringSource = new StringSource(str);
            stringSource.Read();
            //validate
            Assert.Equal('e', stringSource.character);
            Assert.Equal(1, stringSource.Line);
            Assert.Equal(1, stringSource.Column);
            Assert.False(stringSource.isEnd);
        }

        [Fact]
        public void StringWithContent_stringSourceReadsThirdCharacter()
        {
            //prepare
            var str = "example";
            //act
            var stringSource = new StringSource(str);
            stringSource.Read();
            stringSource.Read();
            stringSource.Read();
            //validate
            Assert.Equal('a', stringSource.character);
            Assert.Equal(1, stringSource.Line);
            Assert.Equal(3, stringSource.Column);
            Assert.False(stringSource.isEnd);
        }

        [Fact]
        public void StringWithContent_stringSourceReadsAllStringToEnd()
        {
            //prepare
            var str = "e";
            //act
            var stringSource = new StringSource(str);
            stringSource.Read();
            stringSource.Read();
            //validate
            Assert.True(stringSource.isEnd);
        }

        [Fact]
        public void StringEmpty_stringSourceReadEmptyString()
        {
            //prepare
            var str = "";
            //act
            var stringSource = new StringSource(str);
            stringSource.Read();
            //validate
            Assert.True(stringSource.isEnd);
        }
    }

    public class FileSourceTest
    {
        [Fact]
        public void FileWithContent_FileSourceReadsFirstCharacter()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example");
            }

            // act
            var fileSource = new FileSource("file.txt");
            fileSource.Read();

            // validate
            Assert.Equal('e', fileSource.character);
            Assert.Equal(1, fileSource.Line);
            Assert.Equal(1, fileSource.Column);
            Assert.False(fileSource.isEnd);
        }

        [Fact]
        public void FileWithContent_FileSourceReadsThirdCharacter()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("example");
            }

            // act
            var fileSource = new FileSource("file.txt");
            fileSource.Read();
            fileSource.Read();
            fileSource.Read();

            // validate
            Assert.Equal('a', fileSource.character);
            Assert.Equal(1, fileSource.Line);
            Assert.Equal(3, fileSource.Column);
            Assert.False(fileSource.isEnd);
        }

        [Fact]
        public void FileWithContent_FileSourceReadsAllTextToEnd()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
                file.WriteLine("e");
            }

            // act
            var fileSource = new FileSource("file.txt");
            fileSource.Read();
            fileSource.Read();
            fileSource.Read();

            // validate
            Assert.True(fileSource.isEnd);
        }

        [Fact]
        public void FileEmpty_FileSourceReadsEmptyFile()
        {
            // prepare
            using (var file = new StreamWriter("file.txt"))
            {
            }

            // act
            var fileSource = new FileSource("file.txt");
            fileSource.Read();

            // validate
            Assert.True(fileSource.isEnd);
        }
    }

}
