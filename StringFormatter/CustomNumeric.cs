namespace System.Text.Formatting {
	// this file contains the custom numeric formatting routines split out from the Numeric.cs file
	unsafe partial class Numeric {
		static void NumberToCustomFormatString(StringBuffer formatter, ref Number number, StringView specifier, CachedCulture culture) {
			// Special: Handle special values
			switch (number.Scale) {
				case ScaleInf: formatter.Append(number.Sign == 0 ? culture.PositiveInfinity : culture.NegativeInfinity); return;
				case ScaleNaN: formatter.Append(culture.NaN); return;
			}
			// Iteration 1: Split by semicolon
			int specifierPositiveEnd = IndexOfSectionSeparator(specifier);
			int specifierNegativeStart = 0, specifierNegativeEnd, specifierZeroStart = 0;
			if (specifierPositiveEnd == -1) {
				specifierPositiveEnd = specifierNegativeEnd = specifier.Length;
			}
			else {
				specifierNegativeStart = specifierPositiveEnd + 1;
				specifierNegativeEnd = IndexOfSectionSeparator(specifier, specifierNegativeStart);
				if (specifierNegativeEnd == -1) {
					specifierNegativeEnd = specifier.Length;
				}
				else {
					specifierZeroStart = specifierNegativeEnd + 1;
				}
			}
			// Special: Handle zero
			if (IsZero(ref number)) {
				FormatCustomFormatString(formatter, ref number, null, specifier, specifierZeroStart, specifier.Length, culture);
				return;
			}
			// Iteration 2: Divide and round number
			int originalScale = number.Scale;
			if (number.Sign == 0) ApplyDivisionAndPrecision(ref number, specifier, 0, specifierPositiveEnd);
			else ApplyDivisionAndPrecision(ref number, specifier, specifierNegativeStart, specifierNegativeEnd);
			// Iteration 3: Count; Iteration 4: Format
			if (IsZero(ref number)) FormatCustomFormatString(formatter, ref number, null, specifier, specifierZeroStart, specifier.Length, culture);
			else if (number.Sign == 0) FormatCustomFormatString(formatter, ref number, originalScale, specifier, 0, specifierPositiveEnd, culture);
			else {
				if (specifierNegativeStart == 0) formatter.Append(culture.NegativeSign);
				FormatCustomFormatString(formatter, ref number, originalScale, specifier, specifierNegativeStart, specifierNegativeEnd, culture);
			}
		}
		static int IndexOfSectionSeparator(StringView specifier) {
			return IndexOfSectionSeparator(specifier, 0);
		}
		static int IndexOfSectionSeparator(StringView specifier, int index) {
			if (index < 0 || index > specifier.Length) throw new ArgumentOutOfRangeException("index");
			char* ptr = specifier.Data;
			for (; index < specifier.Length; index++) {
				switch (ptr[index]) {
					case ';': return index;
					case '\\':
						index++;
						if (index >= specifier.Length) throw new FormatException();
						break;
					case '\'':
						SkipLiteral(specifier, ref index, '\'');
						break;
					case '"':
						SkipLiteral(specifier, ref index, '"');
						break;
				}
			}
			return -1;
		}
		static bool IsZero(ref Number number) {
			char* ptr = number.Digits;
			while (*ptr != '\0') if (*ptr++ != '0') return false;
			return true;
		}
		static void ApplyDivisionAndPrecision(ref Number number, StringView specifier, int index, int end) {
			int deltaScale = 0, scalingSpecifiers = 0;
			int integralDigits = 0, decimalDigits = 0;
			bool decimalFlag = false, exponentialFlag = false;
			char* ptr = specifier.Data;
			for (; index < end; index++) {
				switch (ptr[index]) {
					case '\\':
						if (++index >= end) throw new FormatException();
						break;
					case '\'':
						SkipLiteral(specifier, ref index, '\'');
						break;
					case '"':
						SkipLiteral(specifier, ref index, '"');
						break;
					case '0':
					case '#':
						if (decimalFlag) decimalDigits++;
						else integralDigits++;
						scalingSpecifiers = 0;
						break;
					case '.': decimalFlag = true; deltaScale -= scalingSpecifiers * 3; break;
					case ',': scalingSpecifiers++; break;
					case '%': deltaScale += 2; break;
					case '‰': deltaScale += 3; break;
					case 'E':
					case 'e':
						if (++index >= end) goto exit;
						char tc0 = ptr[index];
						if (tc0 == '+' || tc0 == '-') {
							if (++index >= end) goto exit;
						}
						if (ptr[index] != '0') break;
						exponentialFlag = true;
						for (index++; index < end && ptr[index] == '0'; index++) ;
						index--;
						break;
				}
			}
		exit:
			if (exponentialFlag) {
				number.Scale = integralDigits;
				RoundNumber(ref number, integralDigits + decimalDigits);
			}
			else {
				number.Scale += deltaScale;
				RoundNumber(ref number, number.Scale + decimalDigits);
			}
		}
		static void FormatCustomFormatString(StringBuffer formatter, ref Number number, int? originalScale, StringView specifier, int index, int end, CachedCulture culture) {
			int start = index;
			int integralDigits = 0, integralZeros = 0,
				decimalDigits  = 0, decimalZeros  = 0;
			bool integralHasZeroFlag = false;
			int exponentialZeros = 0;
			bool decimalFlag = false;
			bool commaFlag = false, groupFlag = false;
			char* ptr = specifier.Data;
			for (; index < end; index++) {
				switch (ptr[index]) {
					case '\\':
						if (++index >= end) throw new FormatException();
						break;
					case '\'':
						SkipLiteral(specifier, ref index, '\'');
						break;
					case '"':
						SkipLiteral(specifier, ref index, '"');
						break;
					case '0':
						if (decimalFlag) {
							if (decimalZeros < decimalDigits)
								decimalZeros = decimalDigits;
							decimalDigits++;
							decimalZeros++;
						}
						else {
							integralDigits++;
							integralZeros++;
							integralHasZeroFlag = true;
							if (commaFlag) groupFlag = true;
						}
						break;
					case '#':
						if (decimalFlag) {
							decimalDigits++;
						}
						else {
							integralDigits++;
							if (integralHasZeroFlag) integralZeros++;
							if (commaFlag) groupFlag = true;
						}
						break;
					case '.': decimalFlag = true; commaFlag = false; break;
					case ',': commaFlag = true; break;
					case 'E':
					case 'e':
						if (++index >= end) goto exit;
						char tc0 = ptr[index];
						if (tc0 == '+' || tc0 == '-') {
							if (++index >= end) goto exit;
						}
						if (ptr[index] != '0') break;
						for (; index < end && ptr[index] == '0'; index++) exponentialZeros++;
						index--;
						break;
				}
			}
		exit:
			int currentDigitIndex = 0;
			int groupIndex = 0, remainingDigitsInGroup = Math.Max(number.Scale, decimalZeros);
			if (groupFlag) while (true) {
				int groupSize = culture.NumberData.GroupSizes.ElementAtOrLast(groupIndex);
				if (remainingDigitsInGroup <= groupSize) break;
				remainingDigitsInGroup -= groupSize;
				groupIndex++;
			}
			bool appendingDigitFlag = false, outOfPrecisionFlag = false;
			decimalFlag = false;
			for (index = start; index < end; index++) {
				switch (ptr[index]) {
					case '\\':
						if (++index >= end) throw new FormatException();
						formatter.Append(ptr[index]);
						break;
					case '\'':
						formatter.AppendLiteral(specifier, ref index, '\'');
						break;
					case '"':
						formatter.AppendLiteral(specifier, ref index, '"');
						break;
					case '0':
					case '#':
						if (!appendingDigitFlag) {
							if (number.Scale > integralDigits) while (currentDigitIndex < number.Scale - integralDigits)
								formatter.AppendIntegralDigit(ref number, ref currentDigitIndex, culture, groupFlag, ref outOfPrecisionFlag, ref groupIndex, ref remainingDigitsInGroup);
							appendingDigitFlag = true;
						}
						if (decimalFlag) {
							char digit = '0';
							if (!outOfPrecisionFlag) {
								if (currentDigitIndex >= number.Precision) outOfPrecisionFlag = true;
								else {
									digit = number.Digits[currentDigitIndex++];
									if (digit == '\0') {
										outOfPrecisionFlag = true;
										digit = '0';
									}
								}
								formatter.Append(digit);
							}
							else if (decimalZeros > 0)
								formatter.Append('0');
							--decimalDigits;
							--decimalZeros;
						}
						else {
							if (integralDigits <= number.Scale)
								formatter.AppendIntegralDigit(ref number, ref currentDigitIndex, culture, groupFlag, ref outOfPrecisionFlag, ref groupIndex, ref remainingDigitsInGroup);
							else if (integralDigits <= integralZeros)
								formatter.AppendIntegralDigit('0', culture, groupFlag, ref groupIndex, ref remainingDigitsInGroup);
							--integralDigits;
						}
						break;
					case '.': formatter.Append(culture.NumberData.DecimalSeparator); decimalFlag = true; break;
					case ',': break;
					case '%': formatter.Append(culture.PercentSymbol); break;
					case '‰': formatter.Append(culture.PerMilleSymbol); break;
					case 'E':
					case 'e':
						char exponentialSymbol = ptr[index];
						if (++index >= end) {
							index--;
							goto default;
						}
						char tc0 = ptr[index];
						bool hasPlusFlag = false;
						if (tc0 == '+' || tc0 == '-') {
							if (++index >= end) {
								index -= 2;
								goto default;
							}
							if (tc0 == '+') hasPlusFlag = true;
						}
						if (ptr[index] != '0') {
							index -= 2;
							goto default;
						}
						for (index++; index < end && ptr[index] == '0'; index++) ;
						index--;

						int exp = originalScale == null ? 0 : originalScale.Value - number.Scale;
						formatter.Append(exponentialSymbol);
						if (exp < 0) {
							formatter.Append(culture.NegativeSign);
							exp = -exp;
						}
						else if (hasPlusFlag) {
							formatter.Append(culture.PositiveSign);
						}
						Int32ToDecStr(formatter, exp, exponentialZeros, "");
						break;
					default:
						formatter.Append(ptr[index]);
						break;
				}
			}
		}
		static void AppendIntegralDigit(this StringBuffer self, ref Number number, ref int index, CachedCulture culture, bool groupFlag, ref bool outOfPrecisionFlag, ref int groupIndex, ref int remainingDigitsInGroup) {
			char digit = '0';
			if (!outOfPrecisionFlag) {
				if (index >= number.Precision) outOfPrecisionFlag = true;
				else {
					digit = number.Digits[index];
					if (digit == '\0') {
						outOfPrecisionFlag = true;
						digit = '0';
					}
				}
			}
			self.AppendIntegralDigit(digit, culture, groupFlag, ref groupIndex, ref remainingDigitsInGroup);
			index++;
		}
		static void AppendIntegralDigit(this StringBuffer self, char digit, CachedCulture culture, bool groupFlag, ref int groupIndex, ref int remainingDigitsInGroup) {
			self.Append(digit);
			if (!groupFlag) return;
			if (--remainingDigitsInGroup == 0) {
				if (--groupIndex >= 0) {
					remainingDigitsInGroup = culture.NumberData.GroupSizes.ElementAtOrLast(groupIndex);
					self.Append(culture.NumberData.GroupSeparator);
				}
			}
		}
		static T ElementAtOrLast<T>(this T[] arr, int index) {
			if (index >= arr.Length) index = arr.Length - 1;
			return arr[index];
		}
		static void SkipLiteral(StringView specifier, ref int index, char delimiter) {
			while (++index < specifier.Length) {
				if (specifier.Data[index] == delimiter) return;
			}
			throw new FormatException();
		}
		static void AppendLiteral(this StringBuffer self, StringView specifier, ref int index, char delimiter) {
			int start = index + 1;
			while (++index < specifier.Length) {
				if (specifier.Data[index] == delimiter) {
					self.Append(specifier.Data + start, index - start);
					return;
				}
			}
			throw new FormatException();
		}
	}
}
