// Copyright 2011-2012 Chris Patterson
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed 
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations under the License.
namespace OdoyuleRules.Dsl.Parsing
{
    using System;


    public class StringInput :
        Input
    {
        readonly int _column;
        readonly int _line;
        readonly int _offset;
        readonly string _text;

        public StringInput(string text)
            : this(text, 0)
        {
        }

        StringInput(string text, int offset, int line = 1, int column = 1)
        {
            _text = text;
            _offset = offset;
            _line = line;
            _column = column;
        }

        public string Text
        {
            get { return _text; }
        }

        public char Char
        {
            get { return _text[_offset]; }
        }

        public bool IsEnd
        {
            get { return _offset == _text.Length; }
        }

        public int Offset
        {
            get { return _offset; }
        }

        public int Line
        {
            get { return _line; }
        }

        public int Column
        {
            get { return _column; }
        }

        public Input Next()
        {
            if (IsEnd)
                throw new InvalidOperationException("The end of the input text was already reached.");

            return new StringInput(_text, _offset + 1,
                Char == '\n'
                    ? _line + 1
                    : _line,
                Char == '\n'
                    ? 1
                    : _column + 1);
        }

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }

        public override bool Equals(object obj)
        {
            var input = obj as StringInput;
            return input != null && input._text == _text && input._offset == _offset;
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode() ^ _offset.GetHashCode();
        }
    }
}