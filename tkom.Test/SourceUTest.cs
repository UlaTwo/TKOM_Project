using System;
using Xunit;
using tkom.LexerN;
using System.IO;

namespace tkom.Test
{
    public class SourceTest
    {
        [Fact]
        public void StringWithContent_SourceReadsFirstCharacter()
        {
            var str = "example";
            using (var s = new StringReader(str))
            {
                var stringSource = new Source(s);
                stringSource.Read();
                //validate
                Assert.Equal('e', stringSource.character);
                Assert.Equal(1, stringSource.position.Line);
                Assert.Equal(1, stringSource.position.Column);
            }
        }


        [Fact]
        public void StringWithContent_stringSourceReadsThirdCharacter()
        {
            //prepare
            var str = "example";
            //act
            using (var s = new StringReader(str))
            {
                var stringSource = new Source(s);
                stringSource.Read();
                stringSource.Read();
                stringSource.Read();
                //validate
                Assert.Equal('a', stringSource.character);
                Assert.Equal(1, stringSource.position.Line);
                Assert.Equal(3, stringSource.position.Column);
            }
        }

        [Fact]
        public void StringWithContent_stringSourceReadsAllStringToEnd()
        {
            //prepare
            var str = "e";
            //act
            using (var s = new StringReader(str))
            {
                var stringSource = new Source(s);
                stringSource.Read();
                stringSource.Read();
                //validate
                Assert.Equal('\0', stringSource.character);
            }
        }

        [Fact]
        public void StringEmpty_stringSourceReadEmptyString()
        {
            //prepare
            var str = "";
            //act
            using (var s = new StringReader(str))
            {
                var stringSource = new Source(s); 
                stringSource.Read();
                //validate
                Assert.Equal('\0', stringSource.character);
            }
        }
    }

}
