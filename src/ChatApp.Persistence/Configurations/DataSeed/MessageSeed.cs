using ChatApp.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Persistence.Configurations.DataSeed
{
    public class MessageSeed : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasData(
                new Message
                {
                    Id = 1,
                    SenderId = "1",
                    SenderUserName = "Mohamed",
                    RecipientId = "2",
                    RecipientUserName = "Mosaad",
                    Content = "test-one",
                    DateRead = DateTime.UtcNow.AddDays(-1),
                    MessageSend = DateTime.UtcNow.AddDays(-2),
                    SenderDeleted = false,
                    RecipientDeleted = false,
                    IsActive = true
                },
                new Message
                {
                    Id = 2,
                    SenderId = "2",
                    SenderUserName = "Khaled",
                    RecipientId = "1",
                    RecipientUserName = "Ahmed",
                    Content = "test-two",
                    DateRead = DateTime.UtcNow.AddDays(-3),
                    MessageSend = DateTime.UtcNow.AddDays(-4),
                    SenderDeleted = false,
                    RecipientDeleted = false,
                    IsActive = false
                },
                new Message
                {
                    Id = 3,
                    SenderId = "2",
                    SenderUserName = "Hossam",
                    RecipientId = "1",
                    RecipientUserName = "Ali",
                    Content = "test-three",
                    DateRead = DateTime.UtcNow.AddDays(4),
                    MessageSend = DateTime.UtcNow.AddDays(3),
                    SenderDeleted = false,
                    RecipientDeleted = false,
                    IsActive = true
                }
                );
        }
    }
}
