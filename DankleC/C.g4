/*
 [The "BSD licence"]
 Copyright (c) 2013 Sam Harwell
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
    derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

grammar C;

root
    : (lineMarker | function | (declaration (Semi)))* EOF
    ;

function
    : Static? declarationSpecifier declarator scope
    ;

parameterList
    : (parameterDeclaration (Comma parameterDeclaration)*)? (Comma Ellipsis)?
    ;

parameterDeclaration
    : declarationSpecifier (declarator | abstractDeclarator)
    ;

argumentList
    : expression (Comma expression)*
    ;

scope
    : LeftBrace statement* RightBrace
    ;

statement
    : semiStatement Semi
    | scope
    | ifStatement
    | whileStatement
    | forStatement
    | switchStatement
    | labelStatement
    | Semi
    ;

semiStatement
    : returnStatement
    | loopControlStatement
    | expressionStatement
    | gotoStatement
    | declaration
    ;

declaration
    : Static? declarationSpecifier (initDeclarator (Comma initDeclarator)*)?
    ;

declarationSpecifier
    : Typedef? (Const | Extern)* (userType | builtinType)
    ;

initDeclarator
    : declarator (Assign initializer)?
    ;

initializer
    : expression
    | arrayInitializer
    ;

arrayInitializer
    : LeftBrace (initializer (Comma initializer)*)+ Comma? RightBrace
    ;

declarator
    : (Star+ Const?)? directDeclarator
    ;

directDeclarator
    : Identifier
    | LeftParen declarator RightParen
    | directDeclarator LeftBracket Constant RightBracket
    | directDeclarator LeftParen parameterList RightParen
    ;

abstractDeclarator
    : (Star+ Const?)? abstractDirectDeclarator?
    ;

abstractDirectDeclarator
    : LeftParen abstractDeclarator RightParen
    | abstractDirectDeclarator LeftBracket Constant RightBracket
    | abstractDirectDeclarator LeftParen parameterList RightParen
    ;

structOrUnion
    : (Struct | Union) Identifier? LeftBrace structDeclaration+ RightBrace
    | (Struct | Union) Identifier
    ;

structDeclaration
    : declarationSpecifier declarator (Comma declarator)* Semi
    ;

expressionStatement
    : expression
    ;

ifStatement
    : If LeftParen expression RightParen statement (Else statement)?
    ;

switchStatement
    : Switch LeftParen expression RightParen LeftBrace switchBlock* RightBrace
    ;

switchBlock
    : Case Constant Colon statement*
    | Default Colon statement*
    ;

whileStatement
    : While LeftParen expression RightParen statement
    | Do statement While LeftParen expression RightParen Semi
    ;

forStatement
    : For LeftParen stmt1=semiStatement? Semi expression? Semi stmt3=expressionStatement? RightParen body=statement
    ;

loopControlStatement
    : Continue
    | Break
    ;

gotoStatement
    : Goto Identifier
    ;

labelStatement
    : Identifier Colon
    ;

shiftExpression
    : additiveExpression ((LeftShift | RightShift) additiveExpression)*
    ;

relationalExpression
    : shiftExpression ((Less | LessEqual | Greater | GreaterEqual) shiftExpression)*
    ;

equalityExpression
    : relationalExpression ((Equal | NotEqual) relationalExpression)*
    ;

andExpression
    : equalityExpression (And equalityExpression)*
    ;

exclusiveOrExpression
    : andExpression (Caret andExpression)*
    ;

inclusiveOrExpression
    : exclusiveOrExpression (Or exclusiveOrExpression)*
    ;

logicalAndExpression
    : inclusiveOrExpression (AndAnd inclusiveOrExpression)*
    ;

logicalOrExpression
    : logicalAndExpression (OrOr logicalAndExpression)*
    ;

conditionalExpression
    : logicalOrExpression (Question expression Colon conditionalExpression)?
    ;

assignmentExpression
    : conditionalExpression
    | unaryExpression (Assign | PlusAssign | MinusAssign | StarAssign | DivAssign | ModAssign | LeftShiftAssign | RightShiftAssign | OrAssign | XorAssign | AndAssign) assignmentExpression
    ;

returnStatement
    : Return expression?
    ;

lvalue
    : expression
    ;

expression
    : assignmentExpression // (Comma assignmentExpression)*
    ;
    
castExpression
    : LeftParen type RightParen castExpression
    | unaryExpression
    ;

unaryExpression
    : postfixExpression
    | (And | Star | Minus | Not | Tilde) castExpression
    | (PlusPlus | MinusMinus) unaryExpression
    | Sizeof ((LeftParen type RightParen) | unaryExpression)
    ;

postfixExpression
    : primaryExpression partialPostfixExpression*
    ;

partialPostfixExpression
    : PlusPlus
    | MinusMinus
    | LeftParen argumentList? RightParen
    | (Dot | Arrow) Identifier
    | LeftBracket expression RightBracket
    ;

primaryExpression
    : constantExpression
    | variableExpression
    | LeftParen expression RightParen
    ;

constantExpression
    : Constant
    | StringLiteral
    ;

additiveExpression
    : multiplicativeExpression ((Plus | Minus) multiplicativeExpression)*
    ;

multiplicativeExpression
    : castExpression ((Star | Div | Mod) castExpression)*
    ;

variableExpression
    : Identifier
    ;

type
    : declarationSpecifier abstractDeclarator
    ;

userType
    : Identifier
    | structOrUnion
    ;

builtinType
    : Void
    | Bool
    | integerType
    ;

integerType
    : unsignedChar
    | signedChar
    | unsignedShort
    | signedShort
    | unsignedLong
    | signedLong
    | unsignedLongLong
    | signedLongLong
    | float
    | double
    | longDouble
    | unsignedInt
    | signedInt
    ;

unsignedChar
    : Char
    | Unsigned Char
    ;

signedChar
    : Signed Char
    ;

unsignedShort
    : Unsigned Short
    | Unsigned Short Int
    ;

signedShort
    : Short
    | Short Int
    | Signed Short
    | Signed Short Int
    ;

unsignedInt
    : Unsigned Int
    | Unsigned
    ;

signedInt
    : Int
    | Signed Int
    | Signed
    ;

unsignedLong
    : Unsigned Long
    | Unsigned Long Int
    ;

signedLong
    : Long
    | Long Int
    | Signed Long
    | Signed Long Int
    ;

unsignedLongLong
    : Unsigned Long Long
    | Unsigned Long Long Int
    ;

signedLongLong
    : Long Long
    | Long Long Int
    | Signed Long Long
    | Signed Long Long Int
    ;

float
    : Float
    ;

double
    : Double
    ;

longDouble
    : Long Double
    ;

lineMarker
    : Hashtag 'line' Constant StringLiteral
    ;

Auto
    : 'auto'
    ;

Break
    : 'break'
    ;

Case
    : 'case'
    ;

Char
    : 'char'
    ;

Const
    : 'const'
    ;

Continue
    : 'continue'
    ;

Default
    : 'default'
    ;

Do
    : 'do'
    ;

Double
    : 'double'
    ;

Else
    : 'else'
    ;

Enum
    : 'enum'
    ;

Extern
    : 'extern'
    ;

Float
    : 'float'
    ;

For
    : 'for'
    ;

Goto
    : 'goto'
    ;

If
    : 'if'
    ;

Inline
    : 'inline'
    ;

Int
    : 'int'
    ;

Long
    : 'long'
    ;

Register
    : 'register'
    ;

Restrict
    : 'restrict'
    ;

Return
    : 'return'
    ;

Short
    : 'short'
    ;

Signed
    : 'signed'
    ;

Sizeof
    : 'sizeof'
    ;

Static
    : 'static'
    ;

Struct
    : 'struct'
    ;

Switch
    : 'switch'
    ;

Typedef
    : 'typedef'
    ;

Union
    : 'union'
    ;

Unsigned
    : 'unsigned'
    ;

Void
    : 'void'
    ;

Volatile
    : 'volatile'
    ;

While
    : 'while'
    ;

Alignas
    : '_Alignas'
    ;

Alignof
    : '_Alignof'
    ;

Atomic
    : '_Atomic'
    ;

Bool
    : '_Bool'
    ;

Complex
    : '_Complex'
    ;

Generic
    : '_Generic'
    ;

Imaginary
    : '_Imaginary'
    ;

Noreturn
    : '_Noreturn'
    ;

StaticAssert
    : '_Static_assert'
    ;

ThreadLocal
    : '_Thread_local'
    ;

LeftParen
    : '('
    ;

RightParen
    : ')'
    ;

LeftBracket
    : '['
    ;

RightBracket
    : ']'
    ;

LeftBrace
    : '{'
    ;

RightBrace
    : '}'
    ;

Less
    : '<'
    ;

LessEqual
    : '<='
    ;

Greater
    : '>'
    ;

GreaterEqual
    : '>='
    ;

LeftShift
    : '<<'
    ;

RightShift
    : '>>'
    ;

Plus
    : '+'
    ;

PlusPlus
    : '++'
    ;

Minus
    : '-'
    ;

MinusMinus
    : '--'
    ;

Star
    : '*'
    ;

Div
    : '/'
    ;

Mod
    : '%'
    ;

And
    : '&'
    ;

Or
    : '|'
    ;

AndAnd
    : '&&'
    ;

OrOr
    : '||'
    ;

Caret
    : '^'
    ;

Not
    : '!'
    ;

Tilde
    : '~'
    ;

Question
    : '?'
    ;

Colon
    : ':'
    ;

Semi
    : ';'
    ;

Comma
    : ','
    ;

Assign
    : '='
    ;

// '*=' | '/=' | '%=' | '+=' | '-=' | '<<=' | '>>=' | '&=' | '^=' | '|='
StarAssign
    : '*='
    ;

DivAssign
    : '/='
    ;

ModAssign
    : '%='
    ;

PlusAssign
    : '+='
    ;

MinusAssign
    : '-='
    ;

LeftShiftAssign
    : '<<='
    ;

RightShiftAssign
    : '>>='
    ;

AndAssign
    : '&='
    ;

XorAssign
    : '^='
    ;

OrAssign
    : '|='
    ;

Equal
    : '=='
    ;

NotEqual
    : '!='
    ;

Arrow
    : '->'
    ;

Dot
    : '.'
    ;

Ellipsis
    : '...'
    ;

Hashtag
    : '#'
    ;

Identifier
    : IdentifierNondigit (IdentifierNondigit | Digit)*
    ;

fragment IdentifierNondigit
    : Nondigit
    | UniversalCharacterName
    //|   // other implementation-defined characters...
    ;

fragment Nondigit
    : [a-zA-Z_]
    ;

fragment Digit
    : [0-9]
    ;

fragment UniversalCharacterName
    : '\\u' HexQuad
    | '\\U' HexQuad HexQuad
    ;

fragment HexQuad
    : HexadecimalDigit HexadecimalDigit HexadecimalDigit HexadecimalDigit
    ;

Constant
    : IntegerConstant
    | FloatingConstant
    //|   EnumerationConstant
    | CharacterConstant
    ;

fragment IntegerConstant
    : DecimalConstant IntegerSuffix?
    | OctalConstant IntegerSuffix?
    | HexadecimalConstant IntegerSuffix?
    | BinaryConstant
    ;

fragment BinaryConstant
    : '0' [bB] [0-1]+
    ;

fragment DecimalConstant
    : '-'? NonzeroDigit Digit*
    ;

fragment OctalConstant
    : '0' OctalDigit*
    ;

fragment HexadecimalConstant
    : HexadecimalPrefix HexadecimalDigit+
    ;

fragment HexadecimalPrefix
    : '0' [xX]
    ;

fragment NonzeroDigit
    : [1-9]
    ;

fragment OctalDigit
    : [0-7]
    ;

fragment HexadecimalDigit
    : [0-9a-fA-F]
    ;

fragment IntegerSuffix
    : UnsignedSuffix LongSuffix?
    | LongSuffix UnsignedSuffix?
    ;

fragment UnsignedSuffix
    : [uU]
    ;

fragment LongSuffix
    : [lL]
    ;

fragment FloatingConstant
    : DecimalFloatingConstant
    | HexadecimalFloatingConstant
    ;

fragment DecimalFloatingConstant
    : FractionalConstant ExponentPart? FloatingSuffix?
    | DigitSequence ExponentPart FloatingSuffix?
    ;

fragment HexadecimalFloatingConstant
    : HexadecimalPrefix (HexadecimalFractionalConstant | HexadecimalDigitSequence) BinaryExponentPart FloatingSuffix?
    ;

fragment FractionalConstant
    : DigitSequence? '.' DigitSequence
    | DigitSequence '.'
    ;

fragment ExponentPart
    : [eE] Sign? DigitSequence
    ;

fragment Sign
    : [+-]
    ;

DigitSequence
    : Digit+
    ;

fragment HexadecimalFractionalConstant
    : HexadecimalDigitSequence? '.' HexadecimalDigitSequence
    | HexadecimalDigitSequence '.'
    ;

fragment BinaryExponentPart
    : [pP] Sign? DigitSequence
    ;

fragment HexadecimalDigitSequence
    : HexadecimalDigit+
    ;

fragment FloatingSuffix
    : [flFL]
    ;

fragment CharacterConstant
    : '\'' CCharSequence '\''
    | 'L\'' CCharSequence '\''
    | 'u\'' CCharSequence '\''
    | 'U\'' CCharSequence '\''
    ;

fragment CCharSequence
    : CChar+
    ;

fragment CChar
    : ~['\\\r\n]
    | EscapeSequence
    ;

fragment EscapeSequence
    : SimpleEscapeSequence
    | OctalEscapeSequence
    | HexadecimalEscapeSequence
    | UniversalCharacterName
    ;

fragment SimpleEscapeSequence
    : '\\' ['"?abfnrtv\\]
    ;

fragment OctalEscapeSequence
    : '\\' OctalDigit OctalDigit? OctalDigit?
    ;

fragment HexadecimalEscapeSequence
    : '\\x' HexadecimalDigit+
    ;

StringLiteral
    : EncodingPrefix? '"' SCharSequence? '"'
    ;

fragment EncodingPrefix
    : 'u8'
    | 'u'
    | 'U'
    | 'L'
    ;

fragment SCharSequence
    : SChar+
    ;

fragment SChar
    : ~["\\\r\n]
    | EscapeSequence
    | '\\\n'   // Added line
    | '\\\r\n' // Added line
    ;

// ignore the following asm blocks:
/*
    asm
    {
        mfspr x, 286;
    }
 */
AsmBlock
    : 'asm' ~'{'* '{' ~'}'* '}' -> channel(HIDDEN)
    ;

Whitespace
    : [ \t]+ -> channel(HIDDEN)
    ;

Newline
    : ('\r' '\n'? | '\n') -> channel(HIDDEN)
    ;

BlockComment
    : '/*' .*? '*/' -> channel(HIDDEN)
    ;

LineComment
    : '//' ~[\r\n]* -> channel(HIDDEN)
    ;