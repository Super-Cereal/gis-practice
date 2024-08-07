﻿using GISServer.API.Model;
using GISServer.Domain.Model;

namespace GISServer.API.Service
{
    public interface IClassifierService
    {
        public ClassifierDTO InitClassifier(ClassifierDTO classifierDTO);
        public Task<ClassifierDTO> AddClassifier(ClassifierDTO classifierDTO);
        public Task<ClassifierDTO> GetClassifier(Guid id);
        public Task<List<ClassifierDTO>> GetClassifiers();
        public Task<(bool, string)> DeleteClassifier(Guid id);
    }
}
