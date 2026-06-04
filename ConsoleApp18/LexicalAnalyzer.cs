using System;

namespace Компилятор
{
    public class LexicalAnalyzer
    {
        private byte _symbol;

        private TextPosition _token;

        private string _addrName;

        private int _nmbInt;

        private string _stringValue;

        private Keywords _keywords;

        private InputOutput _inputOutput;

        public const byte
            plus = 70,
            minus = 71,
            star = 21,
            slash = 60,
            equal = 16,
            comma = 20,
            leftpar = 9,
            rightpar = 4,
            stringc = 83,
            semicolon = 14,
            colon = 5,
            point = 61,
            later = 65,
            laterequal = 67,
            latergreater = 69,
            assign = 51,
            twopoints = 74,
            ident = 2,
            intc = 15,
            casesy = 31,
            elsesy = 32,
            filesy = 57,
            gotosy = 33,
            thensy = 52,
            typesy = 34,
            untilsy = 53,
            dosy = 54,
            withsy = 37,
            ifsy = 56,
            insy = 100,
            ofsy = 101,
            orsy = 102,
            tosy = 103,
            endsy = 104,
            varsy = 105,
            divsy = 106,
            andsy = 107,
            notsy = 108,
            forsy = 109,
            modsy = 110,
            nilsy = 111,
            setsy = 112,
            beginsy = 113,
            whilesy = 114,
            arraysy = 115,
            constsy = 116,
            labelsy = 117,
            downtosy = 118,
            packedsy = 119,
            recordsy = 120,
            repeatsy = 121,
            programsy = 122,
            fieldpoint = 75,
            functionsy = 123,
            procedurensy = 124;

        public LexicalAnalyzer(
            InputOutput inputOutputValue
        )
        {
            _symbol = 0;

            _token =
                new TextPosition(0, 0);

            _addrName = "";

            _nmbInt = 0;

            _stringValue = "";

            _keywords = new Keywords();

            _inputOutput = inputOutputValue;
        }

        public string LastIdentifier
        {
            get
            {
                return _addrName;
                
            }
        }
        public int LastNumber
        {
            get
            {
                return _nmbInt;
            }
        }


        private bool IsLatinLetter(char ch)
        {
            return
                (ch >= 'a' && ch <= 'z') ||
                (ch >= 'A' && ch <= 'Z');
        }

        private bool IsLatinLetterOrDigit(char ch)
        {
            return
                IsLatinLetter(ch) ||
                (ch >= '0' && ch <= '9');
        }

        public byte NextSym()
        {
            if (_inputOutput.Ch == '\0')
            {
                return 0;
            }

            _symbol = 0;

            while (
            char.IsWhiteSpace(_inputOutput.Ch)
            )
            {
                _inputOutput.NextCh();
            }

            if (_inputOutput.Ch == '\0')
            {
                return 0;
            }

            _token.LineNumber =
                _inputOutput
                    .PositionNow
                    .LineNumber;

            _token.CharNumber =
                _inputOutput
                    .PositionNow
                    .CharNumber;

            switch (_inputOutput.Ch)
            {
                case '\'':

                    ParseString();

                    break;

                case '<':

                    _inputOutput.NextCh();

                    if (_inputOutput.Ch == '=')
                    {
                        _symbol = laterequal;

                        _inputOutput.NextCh();
                    }
                    else if (
                        _inputOutput.Ch == '>'
                    )
                    {
                        _symbol = latergreater;

                        _inputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = later;
                    }

                    break;

                case ':':

                    _inputOutput.NextCh();

                    if (_inputOutput.Ch == '=')
                    {
                        _symbol = assign;

                        _inputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = colon;
                    }

                    break;

                case ';':

                    _symbol = semicolon;

                    _inputOutput.NextCh();

                    break;

                case '.':

                    _inputOutput.NextCh();

                    if (_inputOutput.Ch == '.')
                    {
                        _symbol = twopoints;

                        _inputOutput.NextCh();
                    }
                    else
                    {
                        _symbol = fieldpoint;
                    }

                    break;

                case '{':

                    ParseComment();

                    break;

                case '+':

                    _symbol = plus;

                    _inputOutput.NextCh();

                    break;

                case '-':

                    _symbol = minus;

                    _inputOutput.NextCh();

                    break;

                case '*':

                    _symbol = star;

                    _inputOutput.NextCh();

                    break;

                case '/':

                    _symbol = slash;

                    _inputOutput.NextCh();

                    break;

                case '=':

                    _symbol = equal;

                    _inputOutput.NextCh();

                    break;

                case ',':

                    _symbol = comma;

                    _inputOutput.NextCh();

                    break;

                case '(':

                    _symbol = leftpar;

                    _inputOutput.NextCh();

                    break;

                case ')':

                    _symbol = rightpar;

                    _inputOutput.NextCh();

                    break;

                default:

                    if (
                        char.IsDigit(
                            _inputOutput.Ch
                        )
                    )
                    {
                        ParseNumber();
                    }
                    else if (
                        IsLatinLetter(
                            _inputOutput.Ch
                        )
                    )
                    {
                        ParseIdentifier();
                    }
                    else if (
                        char.IsLetter(
                            _inputOutput.Ch
                        )
                    )
                    {
                        _symbol = 0;

                        _inputOutput.Error(
                            2,
                            _inputOutput
                                .PositionNow
                        );

                        while (
                            char.IsLetter(
                                _inputOutput.Ch
                            ) ||
                            char.IsDigit(
                                _inputOutput.Ch
                            )
                        )
                        {
                            _inputOutput.NextCh();
                        }
                    }
                    else
                    {
                        _symbol = 0;

                        _inputOutput.Error(
                            1,
                            _inputOutput
                                .PositionNow
                        );

                        _inputOutput.NextCh();
                    }

                    break;
            }

            if (_symbol == 0)
            {
                return NextSym();
            }

            return _symbol;
        }

        private void ParseNumber()
        {
            byte digit = 0;

            Int16 maxint = Int16.MaxValue;

            _nmbInt = 0;

            while (
                char.IsDigit(
                    _inputOutput.Ch
                )
            )
            {
                digit =
                    (byte)(
                        _inputOutput.Ch - '0'
                    );

                if (
                    _nmbInt < maxint / 10 ||
                    (
                        _nmbInt ==
                        maxint / 10 &&
                        digit <= maxint % 10
                    )
                )
                {
                    _nmbInt =
                        10 * _nmbInt + digit;
                }
                else
                {
                    _inputOutput.Error(
                        203,
                        _inputOutput
                            .PositionNow
                    );

                    _nmbInt = 0;

                    while (
                        char.IsDigit(
                            _inputOutput.Ch
                        )
                    )
                    {
                        _inputOutput.NextCh();
                    }
                }

                _inputOutput.NextCh();
            }

            _symbol = intc;
        }

        private void ParseIdentifier()
        {
            string name = "";

            bool found = false;

            bool tooLong = false;

            while (
                IsLatinLetterOrDigit(
                    _inputOutput.Ch
                )
            )
            {
                name +=
                    char.ToLower(
                        _inputOutput.Ch
                    );

                if (
                    name.Length > 20 &&
                    tooLong == false
                )
                {
                    tooLong = true;

                    _inputOutput.Error(
                        201,
                        _inputOutput
                            .PositionNow
                    );
                }

                _inputOutput.NextCh();
            }

            foreach (
                var item in _keywords.Kw
            )
            {
                if (item.Key == name.Length)
                {
                    if (
                        item.Value
                            .ContainsKey(name)
                    )
                    {
                        _symbol =
                            item.Value[name];

                        found = true;

                        break;
                    }
                }
            }

            if (found == false)
            {
                _symbol = ident;

                _addrName = name;
            }
        }

        private void ParseString()
        {
            _stringValue = "";

            uint startLine =
                _inputOutput
                    .PositionNow
                    .LineNumber;

            _inputOutput.NextCh();

            if (_inputOutput.Ch == '\'')
            {
                _inputOutput.Error(
                    103,
                    _inputOutput
                        .PositionNow
                );

                _inputOutput.NextCh();

                return;
            }

            while (
                _inputOutput.Ch != '\'' &&
                _inputOutput.Ch != '\0' &&
                _inputOutput
                    .PositionNow
                    .LineNumber ==
                startLine
            )
            {
                _stringValue +=
                    _inputOutput.Ch;

                _inputOutput.NextCh();
            }

            if (_inputOutput.Ch == '\'')
            {
                _inputOutput.NextCh();

                _symbol = stringc;
            }
            else
            {
                _inputOutput.Error(
                    102,
                    _inputOutput
                        .PositionNow
                );
            }
        }

        private void ParseComment()
        {
            _inputOutput.NextCh();

            while (
                _inputOutput.Ch != '}' &&
                _inputOutput.Ch != '\0'
            )
            {
                _inputOutput.NextCh();
            }

            if (_inputOutput.Ch == '}')
            {
                _inputOutput.NextCh();
            }
            else
            {
                _symbol = 0;

                _inputOutput.Error(
                    101,
                    _inputOutput
                        .PositionNow
                );
            }
        }
    }
}