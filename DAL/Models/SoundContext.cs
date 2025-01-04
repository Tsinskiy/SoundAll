using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DAL.Models;

public partial class SoundContext : DbContext
{
    public SoundContext()
    {
    }

    public SoundContext(DbContextOptions<SoundContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Medium> Media { get; set; }

    public virtual DbSet<Music> Musics { get; set; }

    public virtual DbSet<MusicGenre> MusicGenres { get; set; }

    public virtual DbSet<MusicPlaylist> MusicPlaylists { get; set; }

    public virtual DbSet<MusicTag> MusicTags { get; set; }

    public virtual DbSet<MusicType> MusicTypes { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagsPlaylist> TagsPlaylists { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPlayList> UserPlayLists { get; set; }

    public virtual DbSet<UserPlayListsMusic> UserPlayListsMusics { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=Sound;Trusted_Connection=True;Encrypt=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Medium>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileType).HasMaxLength(4);

            entity.HasOne(d => d.Music).WithMany(p => p.Media)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK_Media_Music");
        });

        modelBuilder.Entity<Music>(entity =>
        {
            entity.ToTable("Music");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Author).WithMany(p => p.Musics)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Music_Authors");

            entity.HasOne(d => d.Type).WithMany(p => p.Musics)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("FK_Music_MusicType");
        });

        modelBuilder.Entity<MusicGenre>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Genre).WithMany(p => p.MusicGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_MusicGenres_Genres");

            entity.HasOne(d => d.Music).WithMany(p => p.MusicGenres)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK_MusicGenres_Music");
        });

        modelBuilder.Entity<MusicPlaylist>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Music).WithMany(p => p.MusicPlaylists)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK_MusicPlaylists_Music");

            entity.HasOne(d => d.Playlist).WithMany(p => p.MusicPlaylists)
                .HasForeignKey(d => d.PlaylistId)
                .HasConstraintName("FK_MusicPlaylists_Playlists");
        });

        modelBuilder.Entity<MusicTag>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Music).WithMany(p => p.MusicTags)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK_MusicTags_Music");

            entity.HasOne(d => d.Tag).WithMany(p => p.MusicTags)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_MusicTags_Tags");
        });

        modelBuilder.Entity<MusicType>(entity =>
        {
            entity.ToTable("MusicType");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength();
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TagsPlaylist>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Playlist).WithMany(p => p.TagsPlaylists)
                .HasForeignKey(d => d.PlaylistId)
                .HasConstraintName("FK_TagsPlaylists_Playlists");

            entity.HasOne(d => d.Tag).WithMany(p => p.TagsPlaylists)
                .HasForeignKey(d => d.TagId)
                .HasConstraintName("FK_TagsPlaylists_Tags");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountFinishDate).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Surname).HasMaxLength(50);
        });

        modelBuilder.Entity<UserPlayList>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.UserPlayLists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserPlayLists_Users");
        });

        modelBuilder.Entity<UserPlayListsMusic>(entity =>
        {
            entity.ToTable("UserPlayListsMusic");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.HasOne(d => d.Music).WithMany(p => p.UserPlayListsMusics)
                .HasForeignKey(d => d.MusicId)
                .HasConstraintName("FK_UserPlayListsMusic_Music");

            entity.HasOne(d => d.UserPlaylist).WithMany(p => p.UserPlayListsMusics)
                .HasForeignKey(d => d.UserPlaylistId)
                .HasConstraintName("FK_UserPlayListsMusic_UserPlayLists");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
