﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RestGen.Lang.Typescript
{
    public sealed class CodeBuilder
    {
        private readonly StringBuilder _code = new StringBuilder();
        private int _indent;

        public IDisposable DummyBlock()
        {
            return new DummyBlockCode();
        }

        public IDisposable Block(string code, params object[] args)
        {
            Code(code, args);
            return new BlockCode(this);
        }

        public IDisposable BlockTerminated(string code, params object[] args)
        {
            Code(code, args);
            return new BlockCode(this, true);
        }

        public CodeBuilder Code(string code, params object[] args)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            _code.Append(IndentString);
            if (args == null || args.Length == 0)
                _code.Append(code);
            else
                _code.AppendFormat(code, args);
            return this;
        }

        public CodeBuilder Then(string code, params object[] args)
        {
            if (args == null || args.Length == 0)
                _code.Append(code);
            else
                _code.AppendFormat(code, args);
            return this;
        }

        public CodeBuilder Then<T>(IList<T> source, Func<T, string> transform, string separator = ", ")
        {
            for (int i = 0; i < source.Count; i++)
            {
                if (i > 0)
                    _code.Append(separator);
                _code.Append(transform(source[i]));
            }
            return this;
        }

        public IDisposable ThenBlock(string code, params object[] args)
        {
            _code.AppendFormat(code, args);
            return new BlockCode(this);
        }

        public CodeBuilder Line(string code, params object[] args)
        {
            Code(code, args);
            _code.AppendLine();
            return this;
        }

        public CodeBuilder Line()
        {
            _code.AppendLine();
            return this;
        }

        public IDisposable Indent()
        {
            return new IndentCode(this);
        }

        private string IndentString
        {
            get { return new string(' ', _indent); }
        }

        public override string ToString()
        {
            return _code.ToString();
        }

        private sealed class DummyBlockCode : IDisposable
        {
            void IDisposable.Dispose()
            {
            }
        }

        private sealed class BlockCode : IDisposable
        {
            private readonly CodeBuilder _code;
            private readonly bool _terminate;

            internal BlockCode(CodeBuilder code, bool terminate = false)
            {
                _code = code;
                _terminate = terminate;
                _code.Then(" {").Line();
                _code._indent += 4;
            }

            void IDisposable.Dispose()
            {
                _code._indent -= 4;
                if (_terminate)
                    _code.Line("};");
                else
                    _code.Line("}");
            }
        }

        private sealed class IndentCode : IDisposable
        {
            private readonly CodeBuilder _code;

            internal IndentCode(CodeBuilder code)
            {
                _code = code;
                _code._indent += 4;
            }

            void IDisposable.Dispose()
            {
                _code._indent -= 4;
            }
        }
    }
}