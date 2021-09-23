using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FormatZPD.Models;
using Neo4j.Driver;

namespace FormatZPD.Repositories
{
    public class PersonRepo : IPersonRepo
    {
        private readonly IDriver _driver;

        public PersonRepo(IDriver driver)
        {
            _driver = driver;
        }

        public async Task<IEnumerable<Person>> GetPeople()
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            List<Person> people = new List<Person>();
            try
            {
                cursor = await session.RunAsync(@"
MATCH (p:Person) 
RETURN p.id AS id, p.firstname AS firstname, p.lastname AS lastname, p.removed AS removed
");
                people = await cursor.ToListAsync(record =>
                {
                    Person person = new Person()
                    {
                        Id = record["id"].As<string>(),
                        FirstName = record["firstname"].As<string>(),
                        LastName = record["lastname"].As<string>(),
                        Removed = record["removed"].As<bool>(),
                    };

                    return person;
                });
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return people;
        }

        public async Task<Person> GetPersonById(string id)
        {
            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            Person person = null;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (p:Person {id: $id})  
RETURN p.id AS id, p.firstname AS firstname, p.lastname AS lastname, p.removed AS removed
", new {id});
                IRecord record = await cursor.SingleAsync();
                if (record != null)
                {
                    person = new Person()
                    {
                        Id = record["id"].As<string>(),
                        FirstName = record["firstname"].As<string>(),
                        LastName = record["lastname"].As<string>(),
                        Removed = record["removed"].As<bool>(),
                    };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return person;
        }

        public async Task<Person> CreatePerson(Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            IResultCursor cursor;
            IAsyncSession session = _driver.AsyncSession();
            try
            {
                var parameters = new
                {
                    newPerson = person.AsDictionary()
                };
                string query = @"
CREATE (child: Person {firstname: $newPerson.firstname, lastname: $newPerson.lastname, removed: false, id: apoc.create.uuid()})
RETURN child.id AS id
";
                cursor = await session.RunAsync(query,
                    parameters);
                IRecord record = await cursor.SingleAsync();
                person.Id = record["id"].As<string>();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return person;
        }

        public async Task<Person> EditPerson(string id, Person person)
        {
            if (person == null)
            {
                throw new ArgumentNullException(nameof(person));
            }

            IAsyncSession session = _driver.AsyncSession();
            try
            {
                person.Id = id;
                var parameters = new
                {
                    newPerson = person.AsDictionary()
                };
                string query = @"
MATCH (p: Person {id: $newPerson.id})
SET p.firstname = $newPerson.firstname, p.lastname = $newPerson.lastname
";
                await session.RunAsync(query,
                    parameters);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return person;
        }

        public async Task<Person> DeletePerson(string id, Person person)
        {
            IAsyncSession session = _driver.AsyncSession();
            IResultCursor cursor;
            try
            {
                cursor = await session.RunAsync(@"
MATCH (p: Person {id: $id})
SET p.removed = true
WITH p
WHERE NOT (p)-[:hasResult]->(:Belief)
DETACH DELETE p
RETURN count(*) AS count
", new {id});
                IRecord record = await cursor.SingleAsync();

                if (record["count"].As<long>() == 0)
                    person.Removed = true;
                else
                    person = null;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await session.CloseAsync();
            }

            return person;
        }
    }
}