using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

internal class CountingTextWriter : TextWriter
{
    private readonly TextWriter _writer;
    private int _currentLineCharacterCount;
    private int _newLineMatchIndex;

    public CountingTextWriter(TextWriter innerWriter)
    {
        _writer = innerWriter;
        _currentLineCharacterCount = 0;
        _newLineMatchIndex = 0;
    }

    public override Encoding Encoding => _writer.Encoding;

#if NETCOREAPP3_1_OR_GREATER
    [AllowNull]
#endif
    public override string NewLine
    {
        get { return _writer.NewLine; }
        set { _writer.NewLine = value; }
    }

    public TextWriter InnerWriter => _writer;

    public override void Close() => _writer.Close();

#if NETCOREAPP3_0_OR_GREATER
    /// <inheritdoc/>
    public override ValueTask DisposeAsync() => _writer.DisposeAsync();
#endif

    public override void Flush() => _writer.Flush();

    /// <inheritdoc/>
    public override Task FlushAsync() => _writer.FlushAsync();

    public int CurrentLineCharacterCount => _currentLineCharacterCount;

    public override void Write(char value)
    {
        _writer.Write(value);

        // Handle the NewLine
        if(value == NewLine[_newLineMatchIndex])
        {
            _newLineMatchIndex++;
            if(_newLineMatchIndex == NewLine.Length)
            {
                _currentLineCharacterCount = 0;
                _newLineMatchIndex = 0;
                return;
            }
        }
        else
        {
            _newLineMatchIndex = value == NewLine[0] ? 1 : 0;
        }

        // Count the cha caractères that take space (exclude countrol char except tabulation)
        if(value == '\t' | !char.IsControl(value))
        {
            _currentLineCharacterCount++;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if(disposing)
        {
            _writer.Dispose();
        }
        base.Dispose(disposing);
    }
}