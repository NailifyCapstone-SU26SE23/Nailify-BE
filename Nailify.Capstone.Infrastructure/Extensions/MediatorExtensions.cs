using MediatR;
using Microsoft.EntityFrameworkCore;
using Nailify.Capstone.Application.Common.Models;
using Nailify.Capstone.Domain.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nailify.Capstone.Infrastructure.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext context)
        {
            var domainEntities = context.ChangeTracker.Entries<IEventEntity>()
                                        .Select(x => x.Entity)
                                        .Where(x => x.GetDomainEvents().Any())
                                        .ToList();
            var domainEvents = domainEntities.SelectMany(x => x.GetDomainEvents()).ToList();
            domainEntities.ForEach(x => x.ClearDomainEvents());
            foreach (var domainEvent in domainEvents)
            {
                // Bọc sự kiện bằng wrapper INotification động
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
                var notification = Activator.CreateInstance(notificationType, domainEvent);
                if (notification != null)
                {
                    await mediator.Publish(notification);
                }
            }
        }
    }
}
