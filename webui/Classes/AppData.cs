using System.Collections.Generic;
using AutoMapper;
using DummyData;
using DummyData.Entities;
using webui.Models;

namespace webui.Classes
{
  public class AppData
  {
    public List<PersonViewModel> GetAllPersons()
    {
      DataApi myDataApi = new DataApi();
      var people = myDataApi.GetAllPersons();

      Mapper.CreateMap<Person, PersonViewModel>();
      List<PersonViewModel> results = Mapper.Map<List<Person>, List<PersonViewModel>>(people);

      return results;
    }

  }
}