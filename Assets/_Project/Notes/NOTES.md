Últimos cambios:
- Se reemplazó el uso de TogglePlayerCapsule por EnablePlayerControllers. Esto permite desactivar todos los scripts que permiten interactuar con el jugador SIN apagar el game object player capsule. De esta forma se puede acceder al mismo para cambiar su transform durante cut scenes o cualquier otro momento en el que el jugador no está disponible.
- Se agregó un cut scene para el suicidio de fernandez, además de un nuevo modelo y un video para su televisor.

TOFIX:
- Si bien largamos el control del jugador, siguen funcionando scripts como zoom u observacion. Deberiamos desactivar TODOS los raycasters y controles de cámara también.
- Aumentar tiempo de apurtura y cierre de puerta de microondas. Posiblemente se tendrá que modificar ambos audios usados.
- Pizza Time: agrandar el collider de la mesa de forma tal que el jugador si o si vea el cartel de interacción y sea más intuitivo.
- Pizza Time: Agregar comentario luego de que pasa el coche de la policia.
- Lost Signal 1: Luego de que se haya usado la PC, agregar nuevos
textos de observación para la compu no funcionando.
- Fernandez Suicide: Si la puerta del pasillo a la entrada está abierta al momento de iniciar el cut scene, bloquea la visual de la primera cámara. 

- Los textos de textos internos desaparecen rápido si se pisan uno con el otro.


TODO:
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.
- Implementar cambios de Video del Settings Menu
- Mejorar todo el evento del suicidio. Esto incluye mejoras en el cut scene del mismo, agregar sangre en toda la escena y diálogos de Simon, audio (musica y resolver fuentes de ser usadas).

TOCHECK: