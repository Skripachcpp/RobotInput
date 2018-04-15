using System;
using System.Collections.Generic;
using System.Linq;
using WorkingTools.Extensions;

namespace Core
{
    /// <summary>
    /// Поиск последовательности входной цепочки
    /// </summary>
    /// <typeparam name="TAlphabetInput">входной алфавит</typeparam>
    public class SequenceMachine<TAlphabetInput>
    {
        /// <summary>
        /// Искомые последовательности
        /// </summary>
        /// <remarks>key первый символ входной последовательности; 
        /// value массив последовательностей с одинаковыйм первым символом</remarks>
        private readonly Dictionary<TAlphabetInput, Sequence[]> _sequences = new Dictionary<TAlphabetInput, Sequence[]>();

        /// <summary>
        /// Статус автомата
        /// </summary>
        /// <remarks>проверяет и возвращает последовательности совпавшие с входной цепью</remarks>
        private readonly MachineState _state = new MachineState();
        public SequenceMachine(IEnumerable<Sequence> sequences)
        {
            if (sequences == null) throw new ArgumentNullException("sequences");
            var bunched = sequences.Where(sequence => sequence != null).GroupBy(sequence => sequence.Symbols[0]);
            bunched.ForEach(bunch => _sequences.Add(bunch.Key, bunch.ToArray()));
        }


        /// <summary>
        /// Установить входной символ
        /// </summary>
        /// <param name="alphabetInput">входный символ</param>
        /// <returns>true если входный символ, полностью или частично, совпал хоть с одной последовательностью</returns>
        public bool Input(TAlphabetInput alphabetInput)
        {
            if (_sequences.ContainsKey(alphabetInput))
                _state.Add(_sequences[alphabetInput].Select(sequence => new SequenceState(0, sequence)));

            return _state.Input(alphabetInput);
        }




        /// <summary>
        /// Последовательности полностью совпаышие на текущем шаге входной цепочки
        /// </summary>
        public IReadOnlyList<Sequence> Coincided { get { return _state.Coincided; } }

        /// <summary>
        /// Последовательности отсееные на текущем шаге входной цепочки
        /// </summary>
        public IReadOnlyList<Sequence> ScreenedOut { get { return _state.ScreenedOut; } }




        public class MachineState
        {
            private readonly List<Sequence> _coincided = new List<Sequence>();
            private readonly List<Sequence> _screenedOut = new List<Sequence>();

            private readonly List<SequenceState> _sequences = new List<SequenceState>();

            public void Add(IEnumerable<SequenceState> sequences)
            {
                _sequences.AddRange(sequences);
            }

            public bool Input(TAlphabetInput alphabetInput)
            {
                if (_sequences == null)
                    return false;

                _coincided.Clear();
                bool coincidedAny = false;
                for (int i = _sequences.Count - 1; i >= 0; i--)
                {
                    var sequence = _sequences[i];

                    if (sequence == null)
                        continue;

                    if (sequence.Input(alphabetInput))
                    {
                        coincidedAny = true;

                        if (sequence.End)
                        {
                            _coincided.Add(sequence.Sequence);
                            _sequences.RemoveAt(i);
                        }
                    }
                    else
                    {
                        _screenedOut.Add(sequence.Sequence);
                        _sequences.RemoveAt(i);
                    }
                }

                return coincidedAny;
            }

            /// <summary>
            /// Последовательности полностью совпаышие на текущем шаге входной цепочки
            /// </summary>
            public IReadOnlyList<Sequence> Coincided { get { return _coincided; } }

            /// <summary>
            /// Последовательности отсееные на текущем шаге входной цепочки
            /// </summary>
            public IReadOnlyList<Sequence> ScreenedOut { get { return _screenedOut; } }

        }

        public class SequenceState
        {
            private readonly Sequence _sequence;
            private int _currentIndex;

            public SequenceState(int currentIndex, Sequence sequence)
            {
                _sequence = sequence;
                _currentIndex = currentIndex - 1;
                End = false;
            }

            /// <summary>
            /// Установить входной символ
            /// </summary>
            /// <param name="input">входный символ</param>
            /// <returns></returns>
            public bool Input(TAlphabetInput input)
            {
                _currentIndex++;
                if (_sequence.Symbols.Length <= _currentIndex)
                    throw new IndexOutOfRangeException("длина последовательности меньше днлины входной цепочки");

                if (Equals(input, _sequence.Symbols[_currentIndex]))
                {
                    End = _currentIndex >= _sequence.Symbols.Length -1;
                    return true;
                }

                return false;
            }

            public bool End { get; private set; }
            public Sequence Sequence { get { return _sequence; } }
        }

        public class Sequence
        {
            public Sequence(TAlphabetInput[] symbols)
            {
                if (symbols == null) throw new ArgumentNullException("symbols", "последовательность символов отсутствует");
                if (symbols.Length <= 0) throw new ArgumentOutOfRangeException("symbols", "последователность символов должна содердать хотябы один элемент");

                Symbols = symbols;
            }

            public TAlphabetInput[] Symbols { get; protected set; }
        }
    }
}
