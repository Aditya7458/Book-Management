using Microsoft.AspNetCore.Mvc; 
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;

    public BooksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
    {
        return await _context.Books.ToListAsync();
    }

    [HttpGet("{isbn}")]
    public async Task<ActionResult<Book>> GetBook(string isbn)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

        if (book == null)
            return NotFound();

        return book;
    }

    [HttpPost]
    public async Task<ActionResult<Book>> AddBook(Book book)
    {
        if (await _context.Books.AnyAsync(b => b.ISBN == book.ISBN))
            return Conflict("A book with the same ISBN already exists.");

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { isbn = book.ISBN }, book);
    }

    [HttpPut("{isbn}")]
    public async Task<IActionResult> UpdateBook(string isbn, Book updatedBook)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

        if (book == null)
            return NotFound();

        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.PublicationYear = updatedBook.PublicationYear;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{isbn}")]
    public async Task<IActionResult> DeleteBook(string isbn)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);

        if (book == null)
            return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
