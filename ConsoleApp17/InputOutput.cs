using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


    internal class InputOutput
    {
        private string[] _lines = null;

        private int _lineIndex = 0;

        private int _charIndex = 0;

        private char _currentChar = '\0';

        private bool _eof = false;

        private List<string> _errors = new List<string>();


        public char CurrentChar
        {
            get
            {
                return _currentChar;
            }
        }

        public bool EOF
        {
            get
            {
                return _eof;
            }
        }

        public List<string> Errors
        {
            get
            {
                return _errors;
            }
        }

        public int Line
        {
            get
            {
                return _lineIndex + 1;
            }
        }

        public int Column
        {
            get
            {
                return _charIndex + 1;
            }
        }


        public InputOutput(string path)
        {
            try
            {
                _lines = File.ReadAllLines(path);

                if (_lines.Length == 0)
                {
                    _errors.Add("Ошибка 2: Файл пуст.");

                    _eof = true;

                    return;
                }

                _lineIndex = 0;

                _charIndex = -1;

                _eof = false;

                NextCh();
            }
            catch
            {
                _errors.Add("Ошибка 1: Ошибка открытия файла.");

                _eof = true;
            }
        }


        public void NextCh()
        {
            bool needNextLine = false;

            if (_eof)
            {
                return;
            }

            _charIndex++;

            while (true)
            {
                if (_lineIndex >= _lines.Length)
                {
                    _eof = true;

                    _currentChar = '\0';

                    return;
                }

                if (_charIndex >= _lines[_lineIndex].Length)
                {
                    needNextLine = true;
                }
                else
                {
                    needNextLine = false;
                }

                if (needNextLine)
                {
                    _lineIndex++;

                    _charIndex = 0;
                }
                else
                {
                    break;
                }
            }

            if (_lineIndex < _lines.Length)
            {
                _currentChar = _lines[_lineIndex][_charIndex];

                if (!char.IsLetterOrDigit(_currentChar) &&
                    _currentChar != ' ' &&
                    _currentChar != ';' &&
                    _currentChar != ':' &&
                    _currentChar != '=' &&
                    _currentChar != '+' &&
                    _currentChar != '-' &&
                    _currentChar != '*' &&
                    _currentChar != '/' &&
                    _currentChar != '.' &&
                    _currentChar != ',' &&
                    _currentChar != '(' &&
                    _currentChar != ')' &&
                    _currentChar != '\t')
                {
                    _errors.Add(
                        "Ошибка 4: Недопустимый символ '" +
                        _currentChar +
                        "' в строке " +
                        Line +
                        ", позиция " +
                        Column
                    );
                }
        }
            else
            {
                _eof = true;

                _currentChar = '\0';
            }
        }
    }
