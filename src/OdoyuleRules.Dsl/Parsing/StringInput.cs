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
        public string Text { get; private set; }
        readonly string _text;
        readonly int _offset;
        readonly int _line;
        readonly int _column;

        public StringInput(string text)
            : this(text, 0)
        {
        }

        StringInput(string text, int offset, int line = 1, int column = 1)
        {
            Text = text;

            _text = text;
            _offset = offset;
            _line = line;
            _column = column;
        }

        public Input Advance()
        {
            if (AtEnd)
                throw new InvalidOperationException("The input is already at the end of the source.");

            return new StringInput(_text, _offset + 1, Current == '\n'
                                                           ? _line + 1
                                                           : _line, Current == '\n'
                                                                        ? 1
                                                                        : _column + 1);
        }

        public char Current
        {
            get { return _text[_offset]; }
        }

        public bool AtEnd
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

        public override string ToString()
        {
            return string.Format("Line {0}, Column {1}", _line, _column);
        }

        public override bool Equals(object obj)
        {
            var i = obj as StringInput;
            return i != null && i._text == _text && i._offset == _offset;
        }

        public override int GetHashCode()
        {
            return _text.GetHashCode() ^ _offset.GetHashCode();
        }
    }
}