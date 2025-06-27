using System;
using System.Collections.Generic;

namespace EventPlatform.DTOs
{
    public class EventCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string ImageUrl { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public int TotalTickets { get; set; }
        public List<string> Tags { get; set; }
        public string Mood { get; set; }
    }

    public class EventUpdateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string ImageUrl { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string Location { get; set; }
        public decimal? Price { get; set; }
        public int? TotalTickets { get; set; }
        public List<string> Tags { get; set; }
        public string Mood { get; set; }
    }

    public class EventFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string EventType { get; set; }
        public string SearchTerm { get; set; }
        public string Mood { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string SortBy { get; set; }
    }
}