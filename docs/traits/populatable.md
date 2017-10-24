# Populateable Trait

##  How it works

Each actor can have a population stored in the `Populateable` trait. It performs the following duties:

- Grows the population 
- Terraform the habitat in which the population lifes (adjust the temperature)

## Contributions to domain model

### Command

None

### Exposed Properties

- Population
- Gravity
- SurfaceTemperature
- MiningPercentage

### Utilized Properties

- Associatable.PlayerId
- Associatable.Name

## See Also
