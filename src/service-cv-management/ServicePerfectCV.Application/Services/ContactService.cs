using AutoMapper;
using ServicePerfectCV.Application.Common;
using ServicePerfectCV.Application.DTOs.Contact.Requests;
using ServicePerfectCV.Application.DTOs.Contact.Responses;
using ServicePerfectCV.Application.Exceptions;
using ServicePerfectCV.Application.Interfaces;
using ServicePerfectCV.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ServicePerfectCV.Application.Services
{
    public class ContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ICVRepository _cvRepository;
        private readonly IMapper _mapper;

        public ContactService(
            IContactRepository contactRepository,
            ICVRepository cvRepository,
            IMapper mapper)
        {
            _contactRepository = contactRepository;
            _cvRepository = cvRepository;
            _mapper = mapper;
        }

        public async Task<ContactResponse> UpsertAsync(UpsertContactRequest request)
        {
            var cv = await _cvRepository.GetByIdAsync(request.CVId);
            if (cv == null)
                throw new DomainException(ContactErrors.CVNotFound);

            var existingContact = await _contactRepository.GetByCVIdAsync(request.CVId);

            if (existingContact == null)
            {
                var newContact = _mapper.Map<Contact>(request);

                await _contactRepository.CreateAsync(newContact);
                await _contactRepository.SaveChangesAsync();

                return _mapper.Map<ContactResponse>(newContact);
            }

            // Use extension methods for partial update
            existingContact.UpdateIfPresent(e => e.Email, request.EmailAddress);
            existingContact.UpdateIfPresent(e => e.PhoneNumber, request.Phone);
            existingContact.UpdateIfPresent(e => e.LinkedInUrl, request.LinkedIn);
            existingContact.UpdateIfPresent(e => e.GitHubUrl, request.GitHub);
            existingContact.UpdateIfPresent(e => e.PersonalWebsiteUrl, request.Website);
            existingContact.UpdateIfPresent(e => e.Country, request.CountryName);
            existingContact.UpdateIfPresent(e => e.City, request.CityName);

            _contactRepository.Update(existingContact);
            await _contactRepository.SaveChangesAsync();

            return _mapper.Map<ContactResponse>(existingContact);
        }

        public async Task<ContactResponse> GetByCVIdAsync(Guid cvId)
        {
            var contact = await _contactRepository.GetByCVIdAsync(cvId);
            if (contact == null)
                throw new DomainException(ContactErrors.NotFound);

            return _mapper.Map<ContactResponse>(contact);
        }
    }
}