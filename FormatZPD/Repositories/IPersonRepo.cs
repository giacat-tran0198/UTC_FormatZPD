using System.Collections.Generic;
using System.Threading.Tasks;
using FormatZPD.Models;

namespace FormatZPD.Repositories
{
    public interface IPersonRepo
    {
        public Task<IEnumerable<Person>> GetPeople();
        public Task<Person> GetPersonById(string id);
        public Task<Person> CreatePerson(Person person);
        public Task<Person> EditPerson(string id, Person person);
        public Task<Person> DeletePerson(string id, Person person);
    }
}