- Se ha agregado el manager VFXManager como singleton, que puede ser llamado para iniciar efecto de Headache. El mismo es un stack con el mismo nombre como volumen de post processing, y modificamos entre 0 y 1 su blend desde VFXMananger -> postProccessingHandler

TOFIX - urgente:
- Agregar sonidos para cuando come la pizza.
- Agregar sonidos al tomar escoba y bolsas de basura.
- Agregar cajas bajo la escalera, o algo.
- Hay que agregar iconos con el numero del canal del Walkie Talkie activo por el momento. A futuro tambien cambiar el Control Canvas.
- Tanto las puertas principales como el Player Data Helper deben resetear sus textos a "" al iniciar un nuevo evento, hasta cargar nuevos. Actualmente estan mostrando el ultimo recibido y no concuerda con el hilo narrativo.
- El efecto de Headache funciona perfectamente, se agrego a la PC, la Old Radio, y la TV Evening. Por otro lado, solo se dispara el audio interferencia, pero debe ser remplazado por otro. Quizas el AudioClip sea enviado desde cada ResonanaceZone? El efecto se aplica pero no tiene dinamica, solo aumenta o disminuye en funcion de la distancia, no palpita. Hay que buscar esto, algo pulsante o que al menos se repita entre digamos un rango 0.5 y 1 del blend quizas .. hacer oscilar quizas el tamanio de la vignieta, oscilar el angulo de la camara en x y z , etc .. en funcion del valor del blend / intensidad de resonancia?
- Si dejamos el ResonanceZone en la radio, la PC y la TV Evening, debemos poder apagarlas cuanto menos.

TOFIX:
- DizzinessHandler .. al rotar el jugador mientras se aplica el efecto se exprimentan tirones de giro.

TODO - próximo:
- Definitivamente se tiene que indicar en el menu de pausa lo que el jugador debe ir haciendo. Frente a cualquier distraccion se pierde facilmente el hilo del juego.
- Comenzar a trabajar en evento Headaches.
- Actualizar textos en puertas principales y objetos contemplables al final de la escena de Evening. Por ahora se han dejado de trabajar los SO's.

TODO - despues:
- SISTEMA GUARDADO POR CHECKPOINTS: Dividir las escenas evening y night en escenas mas chicas con menos eventos, y configurar cada escena con los objetos necesarios para usarlas como punto de restauracion de juego.
- Implementar cambios de Video del Settings Menu
- Mostrar Settings Menu Canvas desde el Pause Menu Canvas. Esto implica alterar la lógica del Settings Menu Canvas ya que actualmente al retornar desde cualquier botón "Return" muestra el Main Menu Canvas.

TOCHECK: