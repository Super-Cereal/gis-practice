﻿using GISServer.Domain.Model;
using GISServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GISServer.Infrastructure.Service
{
    public class GeoObjectRepository : IGeoObjectRepository
    {
        private readonly Context _context;

        public GeoObjectRepository(Context context)
        {
            _context = context;
        }

        public async Task<List<GeoObject>> GetGeoObjects()
        {
            return await _context.GeoObjects
                .Include(gnf => gnf.GeoNameFeature)
                .Include(gv => gv.GeometryVersion)
                .Include(g => g.Geometry)
                .Include(goi => goi.GeoObjectInfo)
                .Include(pgo => pgo.ParentGeoObjects)
                .Include(cgo => cgo.ChildGeoObjects)
                .Include(itl => itl.InputTopologyLinks)
                .Include(otl => otl.OutputTopologyLinks)
                .Include(gc => gc.GeoObjectInfo.GeoClassifiers)
                .Include(a => a.Aspects)
                .ToListAsync();
        }

        public async Task<GeoObject> GetGeoObject(Guid id)
        {
            return await _context.GeoObjects
                .Where(go => go.Id == id)
                .Include(gnf => gnf.GeoNameFeature)
                .Include(gv => gv.GeometryVersion)
                .Include(g => g.Geometry)
                .Include(goi => goi.GeoObjectInfo)
                .Include(pgo => pgo.ParentGeoObjects)
                .Include(cgo => cgo.ChildGeoObjects)
                .Include(itl => itl.InputTopologyLinks)
                .Include(otl => otl.OutputTopologyLinks)
                .Include(gc => gc.GeoObjectInfo.GeoClassifiers)
                .Include(a => a.Aspects)
                .FirstOrDefaultAsync();
        }
        public async Task<GeoObject> GetByNameAsync(string name)
        {
            return await _context.GeoObjects
                .Where(o => o.Name == name)
                .Include(l => l.InputTopologyLinks)
                .Include(l2 => l2.OutputTopologyLinks)
                .FirstOrDefaultAsync();
        }
        public async Task<List<GeoObjectsGeoClassifiers>> GetGeoClassifiers(Guid geoObjectId)
        {
            return await _context.GeoObjectsGeoClassifiers
                .Where(o => o.GeoObjectId == geoObjectId)
                .ToListAsync();
        }
        public async Task<List<GeoObjectsGeoClassifiers>> GetGeoObjectsGeoClassifiers(Guid? geoObjectInfoId)
        {
            return await _context.GeoObjectsGeoClassifiers
                .Where(o=>o.GeoObjectId == geoObjectInfoId)
                .Include(gogc => gogc.GeoObjectInfo)
                .Include(gogc => gogc.GeoClassifier)
                .ToListAsync();
        }
        public async Task<List<GeoObjectsGeoClassifiers>> GetGeoObjectsGeoClassifiers()
        {
            return await _context.GeoObjectsGeoClassifiers
                .Include(gogc => gogc.GeoObjectInfo)
                .Include(gogc => gogc.GeoClassifier)
                .ToListAsync();
        }
        public void ChangeTrackerClear()
        {
            _context.ChangeTracker.Clear();
        }

        public async Task<GeoObject> AddGeoObject(GeoObject geoObject)
        {

            await _context.GeoObjects.AddAsync(geoObject);
            await _context.SaveChangesAsync();
            return await GetGeoObject(geoObject.Id);
        }

        public async Task<List<GeoObjectsGeoClassifiers>> AddGeoObjectsGeoClassifiers(GeoObjectsGeoClassifiers geoObjectsGeoClassifiers)
        {
            await _context.GeoObjectsGeoClassifiers.AddAsync(geoObjectsGeoClassifiers);
            await _context.SaveChangesAsync();
            return await GetGeoObjectsGeoClassifiers(geoObjectsGeoClassifiers.GeoObjectId);
        }

        public async Task<GeoObject> UpdateGeoObject(GeoObject geoObject)
        {

            _context.Entry(geoObject).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return geoObject;
        }
        /// ?????? //// // //
        public async Task UpdateAsync(GeoObject geoObject)
        {

            var existgeoObject = GetGeoObject(geoObject.Id).Result;
            if (existgeoObject != null)
            {
                _context.Entry(existgeoObject).CurrentValues.SetValues(geoObject);
                foreach (var InputTopologylink in geoObject.InputTopologyLinks)
                {
                    var existInputTopologylink = existgeoObject.InputTopologyLinks.FirstOrDefault(l => l.Id == InputTopologylink.Id);
                    if (existInputTopologylink == null)
                    {
                        existgeoObject.InputTopologyLinks.Add(InputTopologylink);
                    }
                    else
                    {
                        _context.Entry(existInputTopologylink).CurrentValues.SetValues(InputTopologylink);
                    }
                }
                foreach (var existInputlink in existgeoObject.InputTopologyLinks)
                {
                    if (!geoObject.InputTopologyLinks.Any(l => l.Id == existInputlink.Id))
                    {
                        _context.Remove(existInputlink);
                    }
                }
                foreach (var OutputTopologylink in geoObject.OutputTopologyLinks)
                {
                    var existOutputTopologylink = existgeoObject.OutputTopologyLinks.FirstOrDefault(l => l.Id == OutputTopologylink.Id);
                    if (existOutputTopologylink == null)
                    {
                        existgeoObject.OutputTopologyLinks.Add(OutputTopologylink);
                    }
                    else
                    {
                        _context.Entry(existOutputTopologylink).CurrentValues.SetValues(OutputTopologylink);
                    }
                }
                foreach (var existOutputlink in existgeoObject.OutputTopologyLinks)
                {
                    if (!geoObject.OutputTopologyLinks.Any(l => l.Id == existOutputlink.Id))
                    {
                        _context.Remove(existOutputlink);
                    }
                }

            }
            await _context.SaveChangesAsync();

            /* var existGeoObject = GetGeoObject(geoObject.Id).Result;
             if (existGeoObject != null)
             {
                 _context.Entry(existGeoObject).CurrentValues.SetValues(geoObject);
             }*/

        }
        /*public async void UnionObjects(Guid id_A, Guid id_B)
        {
            var geoObjectA = GetGeoObject(
        }*/

        public async Task<(bool, string)> DeleteGeoObject(Guid id)
        {

            var dbGeoObject = await GetGeoObject(id);
            if (dbGeoObject == null)
            {
                return (false, "GeoObeject could not be found");
            }
            _context.GeoObjects.Remove(dbGeoObject);
            await _context.SaveChangesAsync();
            return (true, "GeoObject got deleted");
        }

        public async Task<List<GeoClassifier>> GetGeoClassifiers()
        {
            return await _context.GeoClassifiers
                .ToListAsync();
        }

        public async Task<GeoClassifier> GetGeoClassifier(Guid? id)
        {
            try{
            return await _context.GeoClassifiers
                .Where(ci => ci.Id == id)
                .FirstOrDefaultAsync();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
            }
        }

        public async Task<GeoClassifier> AddGeoClassifier(GeoClassifier geoClassifier)
        {
            await _context.GeoClassifiers.AddAsync(geoClassifier);
            await _context.SaveChangesAsync();
            return await GetGeoClassifier(geoClassifier.Id);
        }


        public async Task<TopologyLink> GetTopologyLink(Guid? id)
        {
            try
            {
                return await _context.TopologyLinks
                    .Where(tl => tl.Id == id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
            }
        }
        public async Task<List<TopologyLink>> GetTopologyLinks()
        {
            try
            {
                return await _context.TopologyLinks
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return null;
            }
        }

        public async Task<TopologyLink> AddTopologyLink(TopologyLink topologyLink)
        {

            await _context.TopologyLinks.AddAsync(topologyLink);
            await _context.SaveChangesAsync();
            return await GetTopologyLink(topologyLink.Id);
        }

        public async Task<Aspect> GetAspect(Guid? id)
        {
            return await _context.Aspects
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Aspect>> GetAspects()
        {
            return await _context.Aspects
                .ToListAsync();
        }

        public async Task<Aspect> AddAspect(Aspect aspect)
        {
            await _context.AddAsync(aspect);
            await _context.SaveChangesAsync();
            return await GetAspect(aspect.Id);
        }
   
        public async Task<GeoObject> AddGeoObjectAspect(Guid geoObjectId, Guid aspectId)
        {
            _context.Aspects
                .Where(a => a.Id == aspectId)
                .ExecuteUpdate(b =>
                    b.SetProperty(a => a.GeographicalObjectId, geoObjectId)
                );
            return await GetGeoObject(geoObjectId);
        }

        public async Task<List<Aspect>> GetGeoObjectAspects(Guid geoObjectId)
        {
            return await _context.Aspects
                .Where(a => a.GeographicalObjectId == geoObjectId)
                .ToListAsync();
        }
    }
}
