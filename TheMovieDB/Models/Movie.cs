﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace TheMovieDB.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [DisplayName("IMDB")]
        public string? IMDB { get; set; }
        [DisplayName("Genre")]
        public string? Genre { get; set; }
        [DisplayName("Year")]
        public int Year { get; set; }
        [DataType(DataType.Upload)]
        [DisplayName("Poster")]
        public byte[]? Poster { get; set; }
        [DisplayName("Description")]
        public string? Description { get; set; }

        public string? Director { get; set; }


    }
}