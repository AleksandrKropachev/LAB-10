using System;
using System.Collections.Generic;

namespace Компилятор
{
    public class SyntaxAnalyzer
    {
        private LexicalAnalyzer _lexicalAnalyzer;

        private byte _symbol;

        private Dictionary<string, VariableInfo>
            _table;

        private string _currentRecord;

        public SyntaxAnalyzer(
            LexicalAnalyzer lexicalAnalyzerValue
        )
        {
            _lexicalAnalyzer =
                lexicalAnalyzerValue;

            _symbol = 0;

            _table =
                new Dictionary<string, VariableInfo>();

            _currentRecord = "";
        }

        private class VariableInfo
        {
            private string _name;

            private string _type;

            private Dictionary<string, string>
                _fields;

            public string Name
            {
                get
                {
                    return _name;
                }
            }

            public string Type
            {
                get
                {
                    return _type;
                }
            }

            public Dictionary<string, string>
                Fields
            {
                get
                {
                    return _fields;
                }
            }

            public VariableInfo(
                string nameValue,
                string typeValue
            )
            {
                _name = nameValue;

                _type = typeValue;

                _fields =
                    new Dictionary<string, string>();
            }
        }

        public void Analyze()
        {
            NextSym();

            Program();
        }

        private void NextSym()
        {
            _symbol =
                _lexicalAnalyzer.NextSym();
        }

        private void Require(
        byte code,
        ushort errorCode
        )
        {
            if (_symbol == code)
            {
                NextSym();
            }
            else
            {
                Errors.Print(
                    errorCode
                );
            }
        }

        private void Program()
        {
            if (
                _symbol ==
                LexicalAnalyzer.programsy
            )
            {
                NextSym();

                Require(
                    LexicalAnalyzer.ident,
                    301
                );

                Require(
                    LexicalAnalyzer.semicolon,
                    303
                );
            }

            if (
                _symbol ==
                LexicalAnalyzer.varsy
            )
            {
                VarSection();
            }

            CompoundStatement();

            if (
                _symbol ==
                LexicalAnalyzer.fieldpoint
            )
            {
                NextSym();
            }
            else
            {
                Errors.Print(307);
            }
        }

        private void VarSection()
        {
            Require(
                LexicalAnalyzer.varsy,
                311
            );

            while (
                _symbol ==
                LexicalAnalyzer.ident
            )
            {
                VariableDescription();
            }
        }

        private void VariableDescription()
        {
            string name =
                _lexicalAnalyzer.LastIdentifier;

            Require(
                LexicalAnalyzer.ident,
                301
            );

            Require(
                LexicalAnalyzer.colon,
                302
            );

            if (
                _symbol ==
                LexicalAnalyzer.recordsy
            )
            {
                RecordDescription(name);
            }
            else
            {
                SimpleType(name);

                Require(
                    LexicalAnalyzer.semicolon,
                    303
                );
            }
        }

        private void SimpleType(string name)
        {
            string type =
                _lexicalAnalyzer.LastIdentifier;

            Require(
            LexicalAnalyzer.ident,
            301
            );

            if (
            type != "integer" &&
            type != "char" &&
            type != "boolean" &&
            type != "real"
            )
            {
                Errors.Print(404);
            }

            if (
                _table.ContainsKey(name)
            )
            {
                Errors.Print(401);
            }
            else
            {
                _table[name] =
                    new VariableInfo(
                        name,
                        type
                    );
            }
        }

        private void RecordDescription(
            string name
        )
        {
            VariableInfo recordInfo =
                new VariableInfo(
                    name,
                    "record"
                );

            Require(
                LexicalAnalyzer.recordsy,
                310
            );

            string fieldName = "";

            string fieldType = "";

            while (
                _symbol ==
                LexicalAnalyzer.ident
            )
            {
                fieldName =
                    _lexicalAnalyzer
                        .LastIdentifier;

                Require(
                    LexicalAnalyzer.ident,
                    301
                );

                Require(
                    LexicalAnalyzer.colon,
                    302
                );

                fieldType =
                    _lexicalAnalyzer
                        .LastIdentifier;
                if (
                fieldType != "integer" &&
                fieldType != "char" &&
                fieldType != "boolean" &&
                fieldType != "real"
                )
                {
                    Errors.Print(404);
                }

                Require(
                    LexicalAnalyzer.ident,
                    301
                );

                Require(
                    LexicalAnalyzer.semicolon,
                    303
                );

                recordInfo.Fields[fieldName] =
                    fieldType;
            }

            Require(
                LexicalAnalyzer.endsy,
                305
            );

            Require(
                LexicalAnalyzer.semicolon,
                303
            );

            _table[name] = recordInfo;
        }

        private void CompoundStatement()
        {
            Require(
                LexicalAnalyzer.beginsy,
                304
            );

            while (
                _symbol !=
                LexicalAnalyzer.endsy &&
                _symbol != 0
            )
            {
                Statement();
            }

            Require(
                LexicalAnalyzer.endsy,
                305
            );
        }

        private void Statement()
        {
            if (
                _symbol ==
                LexicalAnalyzer.ident
            )
            {
                Assignment();
            }
            else if (
                _symbol ==
                LexicalAnalyzer.beginsy
            )
            {
                CompoundStatement();
            }
            else if (
                _symbol ==
                LexicalAnalyzer.withsy
            )
            {
                WithStatement();
            }
            else
            {
                NextSym();
            }
        }

        private void Assignment()
        {
            string name =
                _lexicalAnalyzer.LastIdentifier;

            Require(
                LexicalAnalyzer.ident,
                301
            );

            if (
                _symbol ==
                LexicalAnalyzer.fieldpoint
            )
            {
                Require(
                    LexicalAnalyzer.fieldpoint,
                    307
                );

                string fieldName =
                    _lexicalAnalyzer
                        .LastIdentifier;

                Require(
                    LexicalAnalyzer.ident,
                    301
                );

                if (
                    _table.ContainsKey(name)
                )
                {
                    if (
                        _table[name]
                        .Fields
                        .ContainsKey(fieldName)
                        == false
                    )
                    {
                        Errors.Print(403);
                    }
                }
            }
            else
            {
                if (_currentRecord != "")
                {
                    if (
                        _table[_currentRecord]
                        .Fields
                        .ContainsKey(name)
                        == false
                    )
                    {
                        Errors.Print(403);
                    }
                }
                else
                {
                    if (
                        _table.ContainsKey(name)
                        == false
                    )
                    {
                        Errors.Print(402);
                    }
                }
            }

            Require(
                LexicalAnalyzer.assign,
                306
            );

            Expression();

            Require(
                LexicalAnalyzer.semicolon,
                303
            );
        }

        private void Expression()
        {
            if (
                _symbol ==
                LexicalAnalyzer.ident
            )
            {
                string name =
                    _lexicalAnalyzer
                        .LastIdentifier;

                if (_currentRecord != "")
                {
                    if (
                        _table[_currentRecord]
                        .Fields
                        .ContainsKey(name)
                        == false
                    )
                    {
                        Errors.Print(403);
                    }
                }
                else
                {
                    if (
                        _table.ContainsKey(name)
                        == false
                    )
                    {
                        Errors.Print(402);
                    }
                }

                NextSym();
            }
            else if (
                _symbol ==
                LexicalAnalyzer.intc
            )
            {
                NextSym();
            }
            else
            {
                Errors.Print(308);

                NextSym();
            }
        }

        private void WithStatement()
        {
            Require(
                LexicalAnalyzer.withsy,
                312
            );

            _currentRecord =
                _lexicalAnalyzer
                    .LastIdentifier;

            Require(
                LexicalAnalyzer.ident,
                301
            );

            Require(
                LexicalAnalyzer.dosy,
                309
            );

            CompoundStatement();

            _currentRecord = "";
        }
    }
}