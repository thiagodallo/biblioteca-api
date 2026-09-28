using BibliotecaApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Repositories;

public class BibliotecaContext(DbContextOptions<BibliotecaContext> options) : DbContext(options)
{
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(autor =>
        {
            autor.Property(a => a.Nome).HasMaxLength(150).IsRequired();
            autor.Property(a => a.Nacionalidade).HasMaxLength(80);
        });

        modelBuilder.Entity<Livro>(livro =>
        {
            livro.Property(l => l.Titulo).HasMaxLength(200).IsRequired();
            livro.Property(l => l.Isbn).HasMaxLength(20).IsRequired();
            livro.HasIndex(l => l.Isbn).IsUnique();
            livro.HasOne(l => l.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(l => l.AutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
