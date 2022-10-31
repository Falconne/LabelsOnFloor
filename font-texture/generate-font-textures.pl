# This script is encoded in UTF-8
use utf8;

use v5.24;
use strict;
use warnings;

# Output all things (console, file) in UTF-8
use open ':std', IO => ':encoding(UTF-8)';

$| = 1;

### NOTE: I switched to Cygwin with UTF-8 turned on in the settings

# Because Windows is also weird
#use Win32;
#Win32::SetConsoleCP(65001);
#Win32::SetConsoleOutputCP(65001);

use Unicode::UCD qw< charinfo charprop charprops_all >;

##################################################################################################

# C# strings default to UTF-16 strings.  We could support up to the entire Unicode set, but the
# UTF-16 set (2 bytes of data or 64K) is already a lot to parse and put into images.

# YaHei takes up a lot, and I think the max Texture2D size is 16K pixels in width/height.
my (%text, %char_row, %char_col, %max_col);
%text = (
    Consolas       => '',
    Unifont_Narrow => '',
    Unifont_Wide   => '',
    YaHei_CJK_Wide => '',
    'Malgun-Gothic_CJK_Wide' => '',
);
$char_row{$_} = 0 for keys %text;
$char_col{$_} = 0 for keys %text;

%max_col = (
    # Maximum width holders for each pane in Combined.png
    Consolas       => 200,
    YaHei_CJK_Wide => 200,
);
$max_col{Unifont_Narrow} = int( $max_col{Consolas} * 1.205 );    # a bit smaller than Consolas
$max_col{Unifont_Wide}   = int( $max_col{Unifont_Narrow} / 2 );  # exactly double the size of narrow chars
$max_col{'Malgun-Gothic_CJK_Wide'} = $max_col{Unifont_Wide};

my @mapping_offsets;
my $console_col   = 0;
my $prev_accepted = -1;
my $prev_key      = '';

# Manually-entered exceptions for certain characters in Unifont that are widely-spaced, for some
# reason.  Or ones marked as Wide in East_Asian_Width, when they actually aren't.  Or things Consolas
# actually can't handle.
my %unifont_wide = (
    (map { hex($_) => 1 } qw<
        058D 058E
        115D 115E
        2057 210E 210F 212E 213F 2140 214C 214F 2182 2188 219C 219D 21F4 21F9 21FF 22D8 22D9 22F2 22F3 22F5 22F6 22FD 22FF
        2300 2316 232C 23CD 23CE
        25EF 2603 262B 262C 26A9 26B6
        2734 2744 2747 274C 274E 2763 2764 27C1 27C3 27C4 27C8 27C9 27D2
        294A 294B 294E 2950 2952 2953 2956 2957 295A 295B 295E 295F 297E 297F 299E 29DA 29DB 29DF 29E0 29EC 29ED 29F4
        2A0C 2A1D 2A20 2A2D 2A2E 2A68 2A69 2A8D 2A8E 2B2C 2B2D 2B30 2D05 2D07
        2D0A 2D0D 2D10 2D13 2D14 2D1B 2D1C 2D20 2D25 2E3A 2E3B 2E43 2E50 2E51
        A64C A64D A65E A684 A685 A698 A699
        A728 A74E A74F A758 A759 A729 A7C2 A7C3 A7FF A92E
    > ),
    (map {     $_  => 1 } (
        0x1CD3..0x1CF6,
        0x2145..0x2149, 0x213A..0x213D, 0x21FA..0x21FC, 0x22B6..0x22B8, 0x22F9..0x22FB, 0x232D..0x2335, 0x237B..0x237E, 0x2381..0x2394,
        0x2397..0x239A, 0x23B2..0x23B6, 0x23C0..0x23CA, 0x23D4..0x23D9, 0x23DB..0x23E7, 0x23E9..0x23FF, 0x2610..0x2612, 0x2616..0x2619,
        0x2622..0x2624, 0x262F..0x2637, 0x2672..0x268F, 0x2692..0x26A0, 0x26A2..0x26A7, 0x26AD..0x26B1, 0x26BF..0x26E1, 0x26E3..0x26FF,
        0x2794..0x2797, 0x27CB..0x27D0, 0x27D5..0x27DE, 0x27E1..0x27E5, 0x27F0..0x27FF,
        0x2900..0x2907, 0x290A..0x2911, 0x2914..0x2937, 0x293A..0x2948,
        0x2962..0x297B, 0x2993..0x2996, 0x29A8..0x29D0, 0x29E2..0x29EA, 0x29FE..0x2A0A, 0x2A33..0x2A3B, 0x2A4E..0x2A65, 0x2A74..0x2A76,
        0x2A78..0x2A8A, 0x2A95..0x2ABE, 0x2ACD..0x2AD2, 0x2AD7..0x2ADD, 0x2ADF..0x2AED, 0x2AF3..0x2AF5, 0x2AF7..0x2AFD, 0x2B00..0x2B05,
        0x2B08..0x2B0C, 0x2B0E..0x2B1A, 0x2B1F..0x2B24, 0x2B32..0x2B4D, 0x2B50..0x2BC8, 0x2BCA..0x2BEF, 0x2BF0..0x2BFE,
        0x2E0E..0x2E11, 0x2E13..0x2E15,
        0xA662..0xA667, 0xA66C..0xA66E, 0xA732..0xA73D, 0xA771..0xA777, 0xA830..0xA839, 0xA8F2..0xA8FE,
        0xD7B0..0xD7FB
    ) ),
);
my %unifont_narrow = (
    (map { hex($_) => 1 } qw< 1FBF 231A 231B 25FD 25FE 2602 2614 2648 26A1 26AA 26AB 27B0 2938 2939 >),
    (map {     $_  => 1 } (0x0588..0x058C, 0x058F..0x1160, 0x1200..0x1CD2, 0x1CF7..0x1CFF, 0x2648..0x2653, 0x2768..0x2775) ),
);
my $unifont_blocks = qr/Superscripts_And_Subscripts|Number_Forms|Latin_Extended_[DE]/;

# CJK glyphs that are missing in the main Malgun/YaHei fonts
my %unifont_cjk_ranges = (
    (map { hex($_) => 1 } qw< FA6C >),
    (map {     $_  => 1 } (0x31B8..0x31BA, 0x9FA6..0x9FB3, 0x9FC4..0x9FC6, 0xFACF..0xFAD1, 0xFAD5..0xFAD7) ),
);
my $unifont_cjk_blocks = qr/^CJK_Compatibility$|Enclosed_CJK|Katakana_Phonetic_Extensions/;

# The main codepoint loop
for (my $cp = 0; $cp <= 0xFFFF; $cp++) {
    my $char = chr($cp);

    ### NOTE: Exact definitions for properties in https://www.unicode.org/versions/Unicode14.0.0/ch03.pdf
    ### The codepoints.net site is also a good reference, where you can plug in codepoints.net/豈 (or
    ### whatever char you want) to get a bunch of info on it.

    ### Filtering ###

    # A lot of these are used for languages that RimWorld doesn't even support, so might as well
    # trim the list down.
    my $block = charprop($cp, 'Block');
    next if $block =~ /^(
        Combining_Diacritical.*|Hebrew|Arabic.*|Syriac.*|Thaana|NKo|Samaritan|
        Mandaic|No_Block|Cherokee|Unified_Canadian_Aboriginal_Syllabics.*|
        Devanagari|Bengali|Gurmukhi|Gujarati|Oriya|Tamil|Telugu|Kannada|Malayalam|
        Ogham|Tagalog|Hanunoo|Buhid|Tagbanwa|Runic|Khmer.*|
        Sinhala|Thai|Lao|Tibetan|Myanmar.*|Ethiopic.*|
        Mongolian|Limbu|Tai_Le|New_Tai_Lue|Bu[gl]inese|Tai_Tham|Sundanese.*|Batak|Lepcha|Ol_Chiki|

        Control.*|Spacing_Modifier_Letters|\w+_Surrogates|Private_Use.*|Specials
    )$/x;

    ### NOTE: Everything is in uppercase, but we will still include lowercase because most Unicode
    ### blocks are in AaBbCc order, and we don't want an excessive amount of mapping.

    # Skip spacing, other oddballs
    my $general_category = charprop($cp, 'General_Category');
    next unless $general_category =~ /
        \w+_(Letter|Number|Punctuation|Symbol)|
        (Nonspacing|Enclosing)_Mark|Format
    /x || $cp == 0x0020;

    # No non-characters or illegal/obscure codepoints
    next if
        charprop($cp, 'Noncharacter_Code_Point') eq 'Yes' ||
        charprop($cp, 'Identifier_Type') =~ /Not_Character|Limited_Use|Default_Ignorable|Exclusion/
    ;

    # No non-printable whitespace (except for the default space)
    next if charprop($cp, 'White_Space') eq 'Yes' && $cp != 0x0020;

    # Nothing deprecated
    next if charprop($cp, 'Deprecated') eq 'Yes';

    # No combining characters: We only support pre-combined Unicode characters
    # (no uncombined logograms, "o" plus umlaut as two characters, etc.)
    next unless
        charprop($cp, 'Canonical_Combining_Class') eq 'Not_Reordered' &&
        charprop($cp, 'Grapheme_Base') eq 'Yes' &&
        # https://www.unicode.org/reports/tr29/#Grapheme_Cluster_Break_Property_Values
        charprop($cp, 'Grapheme_Cluster_Break') =~ /Other|^LV?T?$/  # Hangui syllables must start with a leading consonant
    ;

    # No right-to-left letters (Hebrew/Arabic)
    next if charprop($cp, 'Bidi_Class') =~ /Right_To_Left|Arabic/;

    my $script           = charprop($cp, 'Script');
    my $east_asian_width = charprop($cp, 'East_Asian_Width');
    my $present_in       = charprop($cp, 'Present_In');

    ### Figure out where to write this thing ###

    my $file =
        $block =~ $unifont_blocks || $unifont_narrow{$cp}       ? 'Unifont_Narrow' :
        $unifont_wide{$cp}                                      ? 'Unifont_Wide' :
        $cp <  0x0588 || $script =~ /Latin|Greek|Armenian/      ? 'Consolas' :
        $cp <= 0x20BF && $general_category eq 'Currency_Symbol' ? 'Consolas' :
        'Unifont_Narrow'
    ;

    # Too new for the font
    $file = 'Unifont_Narrow' if $present_in >= 11;

    # Try to give the CJK strokes a less blocky font by using alternate logogram fonts
    if ($east_asian_width =~ /Wide|Fullwidth/ || ($east_asian_width !~ /Narrow|Halfwidth/ && $script =~ /Hangul|^Han$|^Yi$|Hiragana|Katakana|Bopomofo/)) {
        $file =
            $block =~ $unifont_cjk_blocks || $unifont_cjk_ranges{$cp}
                                    ? 'Unifont_Wide' :
            $block =~ /Hangul|Jamo/ ? 'Malgun-Gothic_CJK_Wide' :
            $block =~ /
                CJK|Kangxi|Katakana|Kanbun|Hiragana|Bopomofo|
                Small_Form_Variants|Halfwidth_And_Fullwidth_Forms
            /x                      ? 'YaHei_CJK_Wide' :
            'Unifont_Wide'
        ;

        # Too new for the fonts
        $file = 'Unifont_Wide' if $file eq 'YaHei_CJK_Wide' && $present_in >= 9;
        $file = 'Unifont_Wide' if $present_in >= 13;
    }

    # Incorrectly marked
    if ($file =~ /Unifont/) {
        $file = 'Unifont_Narrow' if $unifont_narrow{$cp};
        $file = 'Unifont_Wide'   if $unifont_wide{$cp} || ($block =~ /^Enclosed|Dingbats|Yijing_Hexagram_Symbols/ && !$unifont_narrow{$cp});
    }

    ### Incrementing and printing ###

    # Change line numbers after $max_col characters, or if we skip a range
    if ($char_col{$file} >= $max_col{$file}) {
        $text{$file} .= "\n";
        # Zero-based row/cols here
        $char_row{$file}++;
        $char_col{$file} = 0;
    }

    # Keep the console output clean as well
    if ($prev_accepted == -1) {
        printf '%04X: ', $cp;
    }
    elsif ($console_col >= 100) {
        printf "\n%04X: ", $cp;
        $console_col = 0;
    }

    # Produce a mapping of all of the offsets to be able to plot each character to its location
    if ($cp - $prev_accepted > 1) {
        # We skipped some numbers, so we need to make a gap entry
        push @mapping_offsets, { codepoint => $prev_accepted + 1, file => '', row => -1, col => -1 };
    }

    unless ($prev_key eq join("|", $cp-1, $file)) {
        # Changed files or skipped some numbers, so mark it down
        push @mapping_offsets, { codepoint => $cp, file => $file, row => $char_row{$file}, col => $char_col{$file} };
    }

    # Print, record, and increment all the things
    print $char;
    $console_col++;

    $text{$file} .= $char;
    $char_col{$file}++;

    $prev_accepted = $cp;
    $prev_key      = join("|", $cp, $file);
}
push @mapping_offsets, { codepoint => $prev_accepted + 1, file => '', row => -1, col => -1 };  # get the last gap to 0xFFFF
say "\n";

# Write to the files
foreach my $file (sort keys %text) {
    say "Writing to $file.txt...";
    open my $txt_fh, '>', "$file.txt" or die "Cannot open $file.txt for writing: $!";
    print $txt_fh $text{$file};
    close $txt_fh;
}

# Depending on the setup, this may be Microsoft-YaHei-UI-Bold or Microsoft-YaHei-Bold-&-Microsoft-YaHei-UI-Bold.
# Check `magick.exe -list font | grep YaHei` to be sure.
my $yahei_font = 'Microsoft-YaHei-Bold-&-Microsoft-YaHei-UI-Bold';

# Run the ImageMagick commands
say "\n=== Commands for ImageMagick ===";
my @combined_order = qw< Consolas Unifont_Narrow Unifont_Wide Malgun-Gothic_CJK_Wide YaHei_CJK_Wide >;

# NOTE: Consolas is the problem child in this whole bunch.  It loves to bounce around the boundaries,
# despite being a "monospace" font.  So, it's the one that gets all of the extra switches.  Like:
#
# -interline-spacing - to try to separate out the vertical bounds of the character square
# -kerning           - ditto for horizonal bounds
# -strokewidth       - to make sure the bottom part is handled right
# -crop              - All fonts get a single line shaved off from the right/bottom, but Consolas needs
#                      extra lines chopped off to compensate for the strokewidth

my @magick        = qw< magick.exe -background transparent -fill white +antialias -pointsize 48 -encoding unicode >;
my @pre_consolas  = qw< -interline-spacing 7 -kerning 2.61 -strokewidth 7 >;

my @color_opts    = qw< -colors 2 -colorspace gray -depth 1 >;
my @post_opts     = qw< -gravity NorthEast -crop +1+1 +repage -set label "" >;
my @post_consolas = qw< -gravity East      -crop +9+0 +repage -set label "" >;

my @commands = (
    [ @magick, '-font', 'Consolas-Bold', @pre_consolas, 'label:@Consolas.txt', @color_opts, @post_consolas, 'Consolas.png'               ],
    [ @magick, '-font', 'Unifont',            'label:@Unifont_Narrow.txt',         @color_opts, @post_opts, 'Unifont_Narrow.png'         ],
    [ @magick, '-font', 'Unifont',            'label:@Unifont_Wide.txt',           @color_opts, @post_opts, 'Unifont_Wide.png'           ],
    [ @magick, '-font', $yahei_font,          'label:@YaHei_CJK_Wide.txt',         @color_opts, @post_opts, 'YaHei_CJK_Wide.png'         ],
    [ @magick, '-font', 'Malgun-Gothic-Bold', 'label:@Malgun-Gothic_CJK_Wide.txt', @color_opts, @post_opts, 'Malgun-Gothic_CJK_Wide.png' ],

    # Test images
    [ @magick, '-font', 'Consolas-Bold',      'label:@Unifont_Narrow.txt', @color_opts, @post_opts, 'TEST-Consolas-Unifont.png' ],
    [ @magick, '-font', $yahei_font,          'label:@Unifont_Wide.txt',   @color_opts, @post_opts, 'TEST-YaHei-Unifont.png'    ],
    [ @magick, '-font', 'Malgun-Gothic-Bold', 'label:@Unifont_Wide.txt',   @color_opts, @post_opts, 'TEST-Malgun-Unifont.png'   ],

    # The final combined texture
    [
        qw< magick.exe -background transparent -gravity NorthWest ( >,
        (map { "$_.png" } grep { $_ ne 'YaHei_CJK_Wide' } @combined_order),
        qw< -append ) YaHei_CJK_Wide.png +append Combined.png >
    ],
);

foreach my $command (@commands) {
    say "> ".join(" ", map { /[@&:]/ ? "'$_'" : $_ } @$command);
    system @$command;
    say '';
}

push @combined_order, 'Combined';
$char_row{Combined} = -1;
$max_col{Combined}  = 0;

# Parse image dimensions
my %xy_dimensions;
foreach my $file (@combined_order) {
    my $command = "magick.exe identify $file.png";
    say "> $command";

    open (my $im_fh, "-|", $command) or die "Cannot open '$command' for piped output: $!";
    my $line = <$im_fh>;
    say $line;
    $xy_dimensions{$file} = [ $1, $2 ] if $line =~ /^\S+ PNG (\d+)x(\d+)/;
    close $im_fh;
}

# Figure out the block pixel offsets
my $y_offset = 0;
foreach my $file (@combined_order) {
    my $xy = $xy_dimensions{$file};
    my $x_offset = $file eq 'YaHei_CJK_Wide' ? $xy_dimensions{Consolas}[0] : 0;
    $y_offset = 0 if $file =~ /YaHei_CJK_Wide|Combined/;

    push @$xy, $x_offset, $y_offset;
    $y_offset += $xy->[1];
}

# Write the Offsets C# file
my $indent = " " x 8;
say "Writing to Offsets.cs...";
open my $cs_fh, '>', "Offsets.cs" or die "Cannot open Offsets.cs for writing: $!";
say $cs_fh "${indent}public static readonly Dictionary<string, BitmapDimensions> FontBitmapDimensions = new() {";
foreach my $file (@combined_order) {
    my @xy = @{ $xy_dimensions{$file} };
    printf $cs_fh "%s    { %-25s new() { RowCount = %3d, ColumnCount = %3d, BlockRectInPixels = new() { x = %5d, y = %5d, width = %5d, height = %5d } } },\n", $indent, '"'.$file.'",', $char_row{$file}+1, $max_col{$file}, @xy[2,3], @xy[0,1];
}
say $cs_fh "${indent}};";
say $cs_fh "${indent}public static readonly SortedDictionary<int, CharOffset> FontBitmapOffsets = new() {";
foreach my $offset (@mapping_offsets) {
    printf $cs_fh
        "%s    { 0x%04X, new() { FontBitmapName = %-25s Row = %3d, Column = %3d } },\n",
        $indent, $offset->{codepoint}, '"'.$offset->{file}.'",', $offset->{row}, $offset->{col}
    ;
}
say $cs_fh "${indent}};";
close $cs_fh;

exit;
