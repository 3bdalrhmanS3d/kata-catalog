
namespace Gen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var bookRepo = new InMemoryRepository<Book>();

            //bookRepo.Add(new Book(1, "Clean Code", "Robert C. Martin", 2008, 3));
            //bookRepo.Add(new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 2));

            //Console.WriteLine("All books:");
            //foreach (var b in bookRepo.GetAll())
            //{
            //    Console.WriteLine(b);
            //}

            //Console.WriteLine("----------");

            //var book1 = bookRepo.GetById(1);
            //Console.WriteLine("Book with Id = 1:");
            //Console.WriteLine(book1);

            //Console.WriteLine("----------");

            //Console.WriteLine("Books with TotalCopies > 2:");
            //var filtered = bookRepo.Find(b => b.TotalCopies > 2);
            //foreach (var b in filtered)
            //{
            //    Console.WriteLine(b);
            //}

            //Console.WriteLine("----------");

            //bool removed = bookRepo.Remove(2);
            //Console.WriteLine($"Removed book 2? {removed}");



            // Methods
            //int x = 10;
            //int y = 20;
            //Console.WriteLine($"Before Swap: x = {x}, y = {y}");

            //LibraryUtils.Swap(ref x, ref y);

            //Console.WriteLine($"After Swap:  x = {x}, y = {y}");
            //Console.WriteLine("--------------");

            //int maxInt = LibraryUtils.GetMax(5, 12);
            //Console.WriteLine($"Max between 5 and 12 = {maxInt}");

            //string maxString = LibraryUtils.GetMax("Ahmed", "Omar"); 
            //Console.WriteLine($"Max between \"Ahmed\" and \"Omar\" = {maxString}");
            //Console.WriteLine("--------------");

            //var bookRepo = new InMemoryRepository<Book>();

            //bookRepo.Add(new Book(1, "Clean Code", "Robert C. Martin", 2008, 3));
            //bookRepo.Add(new Book(2, "The Pragmatic Programmer", "Andrew Hunt", 1999, 1));
            //bookRepo.Add(new Book(3, "Refactoring", "Martin Fowler", 1999, 0));

            //var availableBooksSorted = LibraryUtils.FilterAndSort(
            //    bookRepo.GetAll(),
            //    b => b.AvailableCopies > 0,
            //    b => b.Title
            //);

            //Console.WriteLine("Available books sorted by title:");
            //foreach (var book in availableBooksSorted)
            //{
            //    Console.WriteLine(book);
            //}

            var successResult = Result<int>.Ok(42);
            Console.WriteLine(successResult.IsSuccess);      // True
            Console.WriteLine(successResult.Value);          // 42
            Console.WriteLine(successResult.ErrorMessage);   // null
            Console.WriteLine(successResult);                // من ToString()

            Console.WriteLine("--------------");

            var failResult = Result<string>.Fail("Something went wrong");
            Console.WriteLine(failResult.IsSuccess);         // False
            Console.WriteLine(failResult.Value);             // null / default
            Console.WriteLine(failResult.ErrorMessage);      // Something went wrong
            Console.WriteLine(failResult);
        }
    }

    public interface IRepo<T> where T : IEntity
    {
        void Add(T item);

        bool Remove(int id);

        T? GetById(int id);

        IReadOnlyList<T> GetAll();

        IEnumerable<T> Find(Func<T, bool> predicate);

    }

    public class InMemoryRepository<T> : IRepo<T> where T : class, IEntity, new()
    {
        private readonly List<T> _items = new List<T>();

        public void Add(T item)
        {
            ArgumentNullException.ThrowIfNull(item);

            _items.Add(item);
        }

        public bool Remove(int id)
        {
            var item = _items.FirstOrDefault(x => x.Id == id);

            if(item == null)
            {
                return false;
            }

            _items.Remove(item);

            return true;

        }

        public T? GetById(int id)
        {
            var item = _items.FirstOrDefault( x => x.Id == id);

            if(item == null )
            {
                return null;
            }

            return item;
        }

        public IReadOnlyList<T> GetAll()
        {
            return _items.AsReadOnly();
        }

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return _items.Where(predicate);
        }

    }

    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Book : IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string Author { get; set; }

        public int Year { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public Book()
        {

        }
        public Book(int id, string title, string author, int year, int totalCopies)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
            TotalCopies = totalCopies;
            AvailableCopies = totalCopies;
        }

        public override string ToString()
        {
            return $"{Id}: {Title} by {Author} ({Year}) - Available: {AvailableCopies}/{TotalCopies}";
        }
    }

    public class Member : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public Member()
        {
        }

        public Member(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Id}: {Name} ({Email})";
        }
    }

    public class Loan : IEntity
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int MemberId { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public Loan()
        {
        }

        public Loan(int id, int bookId, int memberId, DateTime loanDate)
        {
            Id = id;
            BookId = bookId;
            MemberId = memberId;
            LoanDate = loanDate;
            ReturnDate = null;
        }

        public override string ToString()
        {
            var status = ReturnDate.HasValue ? $"Returned at {ReturnDate}" : "Not returned yet";
            return $"Loan {Id}: Book {BookId} to Member {MemberId} on {LoanDate} - {status}";
        }
    }

    public static class LibraryUtils
    {   
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static T GetMax<T>(T a, T b) where T : IComparable<T>
        {
            // CompareTo:
            // < 0 => a أصغر من b
            // = 0 => a يساوي b
            // > 0 => a أكبر من b

            return a.CompareTo(b) >= 0 ? a : b;
        }

        public static IEnumerable<T> FilterAndSort<T, TKey>(
            IEnumerable<T> source,
            Func<T, bool> predicate,
            Func<T, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            var filtered = source.Where(predicate);

            var sorted = filtered.OrderBy(keySelector);

            return sorted;
        }
    }

    public class Box<T>
    {
        private T _value;

        public Box(T value)
        {
            _value = value;
        }

        public T GetValue()
        {
            return _value;
        }

        public void SetValue(T value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return $"Value: {_value}";
        }
    }


    public class Result<T>
    {
         public bool IsSuccess { get; }

        public string? ErrorMessage
            { get; }

        public T? Value { get; }

        private Result(bool isSuccess, T? value, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Ok(T value)
        {
            return new Result<T>(true, value, null);
        }
        public static Result<T> Fail(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = "Unknown error";

            return new Result<T>(false, default, errorMessage);
        }

        public override string ToString()
        {
            if (IsSuccess)
            {
                return $"Success: {Value}";
            }

            return $"Failure: {ErrorMessage}";
        }
    }

}
