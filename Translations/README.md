# Galactic Scale UI translations

The original English text is the translation key. Each embedded TSV file contains one key and one translation per line, separated by a tab. Use `\r\n` for line breaks and `\s` for significant spaces at the end of a key or value. Keep placeholders such as `{0}` intact. Blank lines and lines beginning with `#` are ignored.

`GSLocalization` selects the resource by the game's current language LCID. The current table is `zh-CN.tsv` (2052, Simplified Chinese). To add another language, add an embedded TSV file and register its LCID in `GSLocalization.Languages`. Missing entries fall back to the original English text.

UI values and preference keys stay in their original form; only displayed labels, hints, buttons, dropdown entries, and selector entries are translated. The settings page refreshes when the game language changes.
