global using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Orion.DTOs.Character;

namespace Orion.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private static List<Character> characters = new List<Character>{

            new Character(),
            new Character{ Id = 1 ,Name = "Sam" }

        };


        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                // Create a new Character entity using AutoMapper
                var characterEntity = _mapper.Map<Character>(newCharacter);

                // Add the new character to the database using Entity Framework
                _context.Characters.Add(characterEntity);
                await _context.SaveChangesAsync();

                // Get the updated list of characters from the database
                var characters = await _context.Characters.ToListAsync();

                // Map the characters to DTOs using AutoMapper
                var characterDtos = _mapper.Map<List<GetCharacterDto>>(characters);

                // Set the response data
                serviceResponse.Data = characterDtos;
                serviceResponse.Message = "New character added successfully.";
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the process
                serviceResponse.Success = false;
                serviceResponse.Message = "Error adding character: " + ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                // Find the character by ID
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

                if (character == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found.";
                    return serviceResponse;
                }

                // Remove the character from the database
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();

                // Get the updated list of characters from the database
                var characters = await _context.Characters.ToListAsync();

                // Map the characters to DTOs using AutoMapper
                var characterDtos = _mapper.Map<List<GetCharacterDto>>(characters);

                // Set the response data
                serviceResponse.Data = characterDtos;
                serviceResponse.Message = "Character deleted successfully.";
            }
            catch (Exception ex)
            {
                // Handle any errors that occur during the process
                serviceResponse.Success = false;
                serviceResponse.Message = "Error deleting character: " + ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var ServiceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            ServiceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var ServiceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            ServiceResponse.Data = _mapper.Map<GetCharacterDto>(dbCharacter);
            return ServiceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = characters.FirstOrDefault(c => c.Id == updatedCharacter.Id) ?? throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }


            return serviceResponse;
        }
    }
}