﻿using GISServer.API.Model;
using GISServer.API.Mapper;
using GISServer.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace GISServer.API.Service
{
    public class GeoObjectService : IGeoObjectService
    {
        private readonly IGeoObjectRepository _repository;
        private readonly GeoObjectMapper _geoObjectMapper;
        private readonly AspectMapper _aspectMapper;
        private readonly ClassifierMapper _classifierMapper;

        public GeoObjectService(
                IGeoObjectRepository repository, 
                GeoObjectMapper geoObjectMapper, 
                ClassifierMapper classifierMapper,
                AspectMapper aspectMapper)
        {
            _repository = repository;
            _geoObjectMapper = geoObjectMapper;
            _classifierMapper = classifierMapper;
            _aspectMapper = aspectMapper;
        }

        public async Task<List<GeoObjectDTO>> GetGeoObjects()
        {
            try
            {
                List<GeoObject> geoObjectsFromDB = new List<GeoObject>(await _repository.GetGeoObjects());
                List<GeoObjectDTO> geoObjects = new List<GeoObjectDTO>();
                foreach (var geoObject in geoObjectsFromDB)
                {
                    List<GeoObjectsClassifiers> geoObjectsClassifiersFromDB = new List<GeoObjectsClassifiers>(await _repository.GetGeoObjectsClassifiers(geoObject.Id));
                    foreach (var gogc in geoObjectsClassifiersFromDB)
                    {
                        geoObject.GeoObjectInfo.Classifiers.Add(await _repository.GetClassifier(gogc.ClassifierId));

                    }
                
                    geoObjects.Add(await _geoObjectMapper.ObjectToDTO(geoObject));
                }
                return geoObjects;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GeoObjectDTO> GetGeoObject(Guid id)
        {
            try
            {
                GeoObjectDTO geoObject = await _geoObjectMapper.ObjectToDTO(await _repository.GetGeoObject(id));
                
                List<GeoObjectsClassifiers> geoObjectsClassifiersFromDB = new List<GeoObjectsClassifiers>(
                        await _repository.GetGeoObjectsClassifiers(id));

                foreach (var gogc in geoObjectsClassifiersFromDB)
                {
                   geoObject.GeoObjectInfo.Classifiers.Add(
                       await _classifierMapper.ClassifierToDTO(
                           await _repository.GetClassifier(gogc.ClassifierId)));

                }

                return geoObject;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GeoObjectDTO> AddGeoObject(GeoObjectDTO geoObjectDTO)
        {
            try
            {
                GeoObject geoObject = await _geoObjectMapper.DTOToObject(geoObjectDTO);
                return await _geoObjectMapper.ObjectToDTO(await _repository.AddGeoObject(geoObject));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<GeoObjectDTO> UpdateGeoObject(GeoObjectDTO geoObjectDTO)
        {
            try
            {
                GeoObject geoObject = await _geoObjectMapper.DTOToObject(geoObjectDTO);
                await _repository.UpdateGeoObject(geoObject);
                return geoObjectDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<(bool, string)> DeleteGeoObject(Guid id)
        {
            try
            {
                return await _repository.DeleteGeoObject(id);
            }
            catch (Exception ex)
            {
                return (false, $"An error occured. Error Message: {ex.Message}");
            }
        }

        public GeoObjectDTO CreateGuids(GeoObjectDTO geoObjectDTO)
        {
            Guid guid = Guid.NewGuid();
            geoObjectDTO.Id = guid;
            geoObjectDTO.GeoObjectInfo.Id = guid;
            geoObjectDTO.Geometry.Id = guid;

            return geoObjectDTO;
        }

        public async Task<GeoObjectsClassifiersDTO> AddGeoObjectsClassifiers(GeoObjectsClassifiersDTO geoObjectsClassifiersDTO)
        {
            try
            {
                var geoObjectClassifiers = new GeoObjectsClassifiers
                {
                    GeoObjectId = geoObjectsClassifiersDTO.GeoObjectId,
                    ClassifierId = geoObjectsClassifiersDTO.ClassifierId
                };

                await _repository.AddGeoObjectsClassifiers(geoObjectClassifiers);

                return geoObjectsClassifiersDTO;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured. Error Message: {ex.Message}");
                return null;
            }
        }

        public async Task<GeoObjectDTO> AddGeoObjectAspect(Guid geoObjectId, Guid aspectId)
        {
            try
            {
                return await _geoObjectMapper.ObjectToDTO(
                    await _repository.AddGeoObjectAspect(geoObjectId, aspectId)
                    );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured. Error Message: {ex.Message}");
                return null;
            }
        }

        public async Task<List<AspectDTO>> GetGeoObjectAspects(Guid geoObjectId)
        {
            try
            {
                List<AspectDTO> aspectsDTO = new List<AspectDTO>();
                List<Aspect> aspects = await _repository.GetGeoObjectAspects(geoObjectId);
                foreach(var aspect in aspects)
                {
                    aspectsDTO.Add(await _aspectMapper.AspectToDTO(aspect));
                }
                return aspectsDTO;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
