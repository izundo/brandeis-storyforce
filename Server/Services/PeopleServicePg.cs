using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoryForce.Server.Data;
using StoryForce.Shared.Models;

namespace StoryForce.Server.Services
{
    public class PeopleServicePg :  IPeopleService
    {
        private readonly PgDbContext _dbContext;

        public PeopleServicePg(PgDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<Person> GetByEmailOrNameAndYearAsync(string email, string name, int? year)
        {
            if (!string.IsNullOrEmpty(name) && year.HasValue)
            {
                return await _dbContext.Persons.FirstOrDefaultAsync(s => !string.IsNullOrEmpty(s.Name)
                                                                         && s.Name.Equals(name)
                                                                         && s.ClassOfYear.HasValue
                                                                         && s.ClassOfYear.Value == year.Value);
            }

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email))
            {
                return await _dbContext.Persons.FirstOrDefaultAsync(s => !string.IsNullOrEmpty(s.Name)
                                                                         && s.Name.Equals(name)
                                                                         && !string.IsNullOrEmpty(s.Email)
                                                                         && s.Email.Equals(email));
            }

            return null;
        }

        public async Task<Person> CreateAsync(Person person)
        {
            var existingPerson =
                await GetByEmailOrNameAndYearAsync(person.Email, person.Name, person.ClassOfYear);

            if (existingPerson != null)
            {
                return existingPerson;
            }

            var addedEntry = await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();
            return addedEntry.Entity;
        }


        public async Task<List<Person>> GetAsync() =>
            await _dbContext.Persons.ToListAsync();

        public async Task<Person> GetAsync(int id) =>
            await _dbContext.Persons.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<Person>> CreateMultipleAsync(List<Person> entities)
        {
            await _dbContext.Persons.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task UpdateAsync(int id, Person entity)
        {
            var itemToUpdate = await _dbContext.Persons.FindAsync(id);
            if (itemToUpdate == null)
                throw new NullReferenceException();
            itemToUpdate.Id = id;
            _dbContext.Persons.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(Person entity)
        {
            var itemToRemove = await _dbContext.Persons.FindAsync(entity.Id);
            if (itemToRemove == null)
                throw new NullReferenceException();

            _dbContext.Persons.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            var itemToRemove = await _dbContext.Persons.FindAsync(id);
            if (itemToRemove == null)
                throw new NullReferenceException();

            _dbContext.Persons.Remove(itemToRemove);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Person> GetByEmailAsync(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return await _dbContext.Persons.FirstOrDefaultAsync(s => s.Email == email);
            }
            return null;
        }

        public async Task<List<Person>> GetByAllAsync()
        {
            return await _dbContext.Persons.ToListAsync();
        }
    }
}