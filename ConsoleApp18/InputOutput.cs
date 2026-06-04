using System;
using System.Collections.Generic;
using System.IO;

namespace Компилятор
{
    public struct TextPosition
    {
        private uint _lineNumber;

        private byte _charNumber;

        public uint LineNumber
        {
            get
            {
                return _lineNumber;
            }

            set
            {
                _lineNumber = value;
            }
        }

        public byte CharNumber
        {
            get
            {
                return _charNumber;
            }

            set
            {
                _charNumber = value;
            }
        }

        public TextPosition(
            uint lineNumberValue,
            byte charNumberValue
        )
        {
            _lineNumber = lineNumberValue;

            _charNumber = charNumberValue;
        }
    }

    struct Err
    {
        private TextPosition _errorPosition;

        private byte _errorCode;

        public TextPosition ErrorPosition
        {
            get
            {
                return _errorPosition;
            }

            set
            {
                _errorPosition = value;
            }
        }

        public byte ErrorCode
        {
            get
            {
                return _errorCode;
            }

            set
            {
                _errorCode = value;
            }
        }

        public Err(
            TextPosition positionValue,
            byte errorCodeValue
        )
        {
            _errorPosition = positionValue;

            _errorCode = errorCodeValue;
        }
    }

    public class InputOutput
    {
        private const byte ERRMAX = 9;

        private char _ch;

        private TextPosition _positionNow;

        private string _line;

        private byte _lastInLine;

        private List<Err> _err;

        private StreamReader _file;

        private uint _errCount;

        private bool _isEnd;

        public char Ch
        {
            get
            {
                return _ch;
            }
        }

        public TextPosition PositionNow
        {
            get
            {
                return _positionNow;
            }
        }

        public uint ErrorCount
        {
            get
            {
                return _errCount;
            }
        }

        public InputOutput(string path)
        {
            _ch = '\0';

            _positionNow =
                new TextPosition(0, 0);

            _line = "";

            _lastInLine = 0;

            _err = new List<Err>();

            _file = null;

            _errCount = 0;

            _isEnd = false;

            if (File.Exists(path) == false)
            {
                Console.WriteLine(
                    $"Файл {path} не найден!"
                );

                Environment.Exit(0);
            }

            _file = new StreamReader(path);

            ReadNextLine();

            if (_line.Length > 0)
            {
                _ch = _line[0];
            }
        }

        public void NextCh()
        {
            if (_isEnd == true)
            {
                _ch = '\0';

                return;
            }

            _positionNow.CharNumber++;

            if (
                _positionNow.CharNumber >
                _lastInLine
            )
            {
                ListThisLine();

                if (_err.Count > 0)
                {
                    ListErrors();

                    _err.Clear();
                }

                ReadNextLine();

                _positionNow.LineNumber++;

                _positionNow.CharNumber = 0;
            }

            if (_line.Length > 0)
            {
                _ch =
                    _line[
                        _positionNow.CharNumber
                    ];
            }
            else
            {
                _ch = '\0';
            }
        }

        private void ListThisLine()
        {
            if (_line != "\0")
            {
                Console.Write("    ");

                Console.WriteLine(_line);
            }
        }

        private void ReadNextLine()
        {
            if (_file.EndOfStream == false)
            {
                _line = _file.ReadLine();

                if (_line == null)
                {
                    _line = "";
                }

                _line += " ";

                _lastInLine =
                    (byte)(_line.Length - 1);
            }
            else
            {
                End();
            }
        }

        private void End()
        {
            _isEnd = true;

            _line = "\0";

            _ch = '\0';

            if (_file != null)
            {
                _file.Close();
            }
        }

        private void ListErrors()
        {
            string s = "";

            foreach (Err item in _err)
            {
                _errCount++;

                s = "**";

                if (_errCount < 10)
                {
                    s += "0";
                }

                s += $"{_errCount}**";

                while (
                    s.Length - 6 <
                    item
                        .ErrorPosition
                        .CharNumber
                )
                {
                    s += " ";
                }

                s +=
                    $"^ ошибка код " +
                    $"{item.ErrorCode}";

                Console.WriteLine(s);
            }
        }

        public void Error(
            byte errorCode,
            TextPosition position
        )
        {
            if (_err.Count < ERRMAX)
            {
                Err e =
                    new Err(
                        position,
                        errorCode
                    );

                _err.Add(e);
            }
        }
    }
}